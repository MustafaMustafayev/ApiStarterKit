using CORE.Abstract;
using CORE.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CORE.Concrete;

public class TokenResolverService(
    ConfigSettings config,
    IHttpContextAccessor context,
    IEncryptionService encryptionService)
    : ITokenResolverService
{
    public string GetAccessToken()
    {
        return context.HttpContext?.Request.Headers[config.AuthSettings.HeaderName].ToString()![7..]!;
    }

    public string GetRefreshToken()
    {
        return context.HttpContext?.Request.Headers[config.AuthSettings.RefreshTokenHeaderName].ToString()!;
    }

    public Guid? GetUserIdFromToken()
    {
        var token = GetJwtSecurityToken();

        var claim = token.Claims.First(c => c.Type == config.AuthSettings.TokenUserIdKey);
        if (claim == null)
        {
            return null;
        }

        var userId = encryptionService.Decrypt(claim.Value);

        return Guid.Parse(userId);
    }

    public bool IsValidToken()
    {
        var tokenString = GetAccessToken();

        if (string.IsNullOrEmpty(tokenString) || tokenString.Length < 7)
        {
            return false;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = Encoding.ASCII.GetBytes(config.AuthSettings.SecretKey);
        try
        {
            tokenHandler.ValidateToken(tokenString, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GetJwtSecurityToken()
    {
        var tokenString = GetAccessToken();
        return new JwtSecurityToken(tokenString);
    }
}