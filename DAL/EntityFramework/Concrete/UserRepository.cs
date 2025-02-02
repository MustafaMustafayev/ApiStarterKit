using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Context;
using DAL.EntityFramework.GenericRepository;
using DAL.EntityFramework.Utility;
using DTO;
using ENTITIES.Entities;
using ENTITIES.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFramework.Concrete;

public class UserRepository(DataContext dataContext)
    : GenericRepository<User>(dataContext), IUserRepository
{
    public async Task<string?> GetUserSaltAsync(string email)
    {
        var user = await dataContext.Users.AsNoTracking().SingleOrDefaultAsync(m => m.Email == email);
        return user?.Salt;
    }

    public async Task<bool> IsUserExistAsync(string email, Guid? userId)
    {
        return await dataContext.Users.AnyAsync(m => m.Email == email && m.Id != userId);
    }

    public void UpdateUser(User user)
    {
        dataContext.Entry(user).State = EntityState.Modified;
        dataContext.Entry(user).Property(m => m.Password).IsModified = false;
        dataContext.Entry(user).Property(m => m.Salt).IsModified = false;
    }

    public async Task SoftDeleteMultipleAsync(IEnumerable<Guid> userIds)
    {
        await dataContext.Users
            .Where(t => userIds.Contains(t.Id))
            .ExecuteDeleteAsync();
    }
}