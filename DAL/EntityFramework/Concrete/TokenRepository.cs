using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Context;
using DAL.EntityFramework.GenericRepository;
using ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFramework.Concrete;

public class TokenRepository(DataContext dataContext)
    : GenericRepository<Token>(dataContext), ITokenRepository
{
    public async Task<bool> IsValidAsync(string accessToken)
    {
        return await dataContext.Tokens.AnyAsync(t => t.AccessToken == accessToken &&
                                                      t.AccessTokenExpireDate >= DateTime.Now);
    }

    public async Task RemoveUsersActiveTokensAsync(IEnumerable<Guid> userIds)
    {
        await dataContext.Tokens
            .Where(t => userIds.Contains(t.UserId))
            .ExecuteUpdateAsync(m => m.SetProperty(p => p.IsDeleted, true));
    }

    public async Task RemoveUserActiveTokensAsync(Guid userId)
    {
        await dataContext.Tokens
            .Where(t => t.UserId == userId)
            .ExecuteUpdateAsync(m => m.SetProperty(p => p.IsDeleted, true));
    }
}