using DAL.EntityFramework.GenericRepository;
using ENTITIES.Entities;

namespace DAL.EntityFramework.Abstract;

public interface ITokenRepository : IGenericRepository<Token>
{
    Task<bool> IsValidAsync(string accessToken);
    Task RemoveUsersActiveTokensAsync(IEnumerable<Guid> userIds);
    Task RemoveUserActiveTokensAsync(Guid userId);
}