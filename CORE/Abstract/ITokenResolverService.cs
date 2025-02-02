namespace CORE.Abstract;

public interface ITokenResolverService
{
    public string GetAccessToken();
    public string GetRefreshToken();
    public bool IsValidToken();
    public Guid? GetUserIdFromToken();
    public string GenerateRefreshToken();
}