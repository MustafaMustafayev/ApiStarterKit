using DAL.EntityFramework.Context;
using DAL.EntityFramework.Utility;
using DTO;
using ENTITIES.Entities;
using ENTITIES.Entities.Generic;
using ENTITIES.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.EntityFramework.GenericRepository;

public class GenericRepository<TEntity>(DataContext ctx)
    : IGenericRepository<TEntity>
    where TEntity : class
{
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await ctx.AddAsync(entity);
        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entity)
    {
        await ctx.AddRangeAsync(entity);
        return entity;
    }

    public void Delete(TEntity entity)
    {
        ctx.Remove(entity);
    }

    public void SoftDelete(TEntity entity)
    {
        var property = entity.GetType().GetProperty(nameof(Auditable.IsDeleted)) ?? throw new ArgumentException(
            $"""
             The property with type: {entity.GetType()} can not be SoftDeleted, 
             because it doesn't contains {nameof(Auditable.IsDeleted)} property, 
             and did not implemented {typeof(Auditable)}.
             """);

        if (((bool?)property.GetValue(entity)!).Value)
        {
            throw new Exception("This entity was already deleted.");
        }

        property.SetValue(entity, true);
        ctx.Update(entity);
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(filter)
            : await ctx.Set<TEntity>().FirstOrDefaultAsync(filter);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null,
        bool ignoreQueryFilters = false)
    {
        return filter is null
            ? ignoreQueryFilters
                ? await ctx.Set<TEntity>().IgnoreQueryFilters().ToListAsync()
                : await ctx.Set<TEntity>().ToListAsync()
            : ignoreQueryFilters
                ? await ctx.Set<TEntity>().Where(filter).IgnoreQueryFilters().ToListAsync()
                : await ctx.Set<TEntity>().Where(filter).ToListAsync();
    }

    public async Task<TEntity?> GetAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await ctx.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(filter);
    }

    public async Task<TEntity?> GetAsNoTrackingWithIdentityResolutionAsync(Expression<Func<TEntity, bool>> filter)
    {
        return await ctx.Set<TEntity>()
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(filter);
    }

    public TEntity Update(TEntity entity)
    {
        ctx.Update(entity);
        return entity;
    }

    public List<TEntity> UpdateRange(List<TEntity> entity)
    {
        ctx.UpdateRange(entity);
        return entity;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().CountAsync(filter)
            : await ctx.Set<TEntity>().CountAsync(filter);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().AnyAsync(filter)
            : await ctx.Set<TEntity>().AnyAsync(filter);
    }

    public async Task<bool> AllAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().AllAsync(filter)
            : await ctx.Set<TEntity>().AllAsync(filter);
    }

    public async Task<TEntity> GetAsync(Guid id)
    {
        return (await ctx.Set<TEntity>().FindAsync(id))!;
    }

    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
        bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().SingleOrDefaultAsync(filter)
            : await ctx.Set<TEntity>().SingleOrDefaultAsync(filter);
    }

    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().SingleAsync(filter)
            : await ctx.Set<TEntity>().SingleAsync(filter);
    }

    public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> filter, bool ignoreQueryFilters = false)
    {
        return ignoreQueryFilters
            ? await ctx.Set<TEntity>().IgnoreQueryFilters().FirstAsync(filter)
            : await ctx.Set<TEntity>().FirstAsync(filter);
    }

    public async Task<PaginatedList<TEntity>> GetAsGenericListAsync(GenericRequestDto dto)
    {
        var query = ctx.Set<TEntity>().AsQueryable();

        if (dto.Sort is not null)
        {
            query = query.ApplySorting(dto.Sort.FieldName, dto.Sort.Type == ESortType.Descending);
        }

        if (dto.Filters is not null)
        {
            foreach (var filterItem in dto.Filters)
            {
                query = query.ApplyFiltering(
                    filterItem.FieldName,
                    filterItem.IsInRange,
                    filterItem.IsOnlyEquals,
                    filterItem.Value,
                    filterItem.StartValue,
                    filterItem.EndValue);
            }
        }
        return await PaginatedList<TEntity>.CreateAsync(query, dto.Pagination?.PageIndex ?? 0, dto.Pagination?.PageSize ?? 0);
    }
}