using System.Linq.Expressions;

namespace DAL.EntityFramework.Utility;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query;
        }

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, sortBy);
        var lambda = Expression.Lambda(property, param);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var orderByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)orderByMethod.Invoke(null, [query, lambda])!;
    }

    // Apply dynamic filtering
    public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query,
        string fieldName,
        bool isInRange,
        bool isOnlyEquals,
        string? value,
        string? startValue,
        string? endValue)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return query;
        }

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, fieldName);
        var propertyType = property.Type;

        // Handle range filtering if applicable
        if (isInRange && !string.IsNullOrWhiteSpace(startValue) && !string.IsNullOrWhiteSpace(endValue))
        {
            var startValueConverted = Convert.ChangeType(startValue, propertyType);
            var endValueConverted = Convert.ChangeType(endValue, propertyType);

            var greaterThanOrEqual =
                Expression.GreaterThanOrEqual(property, Expression.Constant(startValueConverted, propertyType));
            var lessThanOrEqual =
                Expression.LessThanOrEqual(property, Expression.Constant(endValueConverted, propertyType));

            var rangeExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            var lambda = Expression.Lambda<Func<T, bool>>(rangeExpression, param);

            return query.Where(lambda);
        }

        // Handle normal filtering if applicable
        if (!isInRange && !string.IsNullOrWhiteSpace(value))
        {
            var constant = Expression.Constant(Convert.ChangeType(value, propertyType));
            var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

            if (propertyType == typeof(string) && containsMethod != null)
            {
                Expression filterExpression;

                if (isOnlyEquals)
                {
                    filterExpression = Expression.Equal(property, constant);
                }
                else
                {
                    filterExpression = Expression.Call(property, containsMethod, constant);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, param);
                return query.Where(lambda);
            }
            else
            {
                var equalsExpression = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, param);
                return query.Where(lambda);
            }
        }

        return query;
    }
}