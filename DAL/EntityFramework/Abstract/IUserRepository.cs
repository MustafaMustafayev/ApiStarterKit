using DAL.EntityFramework.GenericRepository;
using DAL.EntityFramework.Utility;
using DTO;
using ENTITIES.Entities;

namespace DAL.EntityFramework.Abstract;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsUserExistAsync(string email, Guid? userId);
    Task<string?> GetUserSaltAsync(string email);
    void UpdateUser(User user);
    Task SoftDeleteMultipleAsync(IEnumerable<Guid> userIds);
}