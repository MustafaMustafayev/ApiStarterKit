﻿using CORE.Abstract;
using CORE.Config;
using DTO.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CORE.Helpers;

public class SecurityHelper(
    ConfigSettings configSettings,
    IEncryptionService encryptionService)
{
    public static string GenerateSalt()
    {
        var saltBytes = new byte[16];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var hashed = KeyDerivation.Pbkdf2(
            password,
            saltBytes,
            KeyDerivationPrf.HMACSHA512,
            100000,
            512 / 8);

        return Convert.ToBase64String(hashed);
    }

    public string CreateTokenForUser(UserResponseDto userDto, DateTime expirationDate)
    {
        var claims = new List<Claim>
        {
            new(configSettings.AuthSettings.TokenUserIdKey, encryptionService.Encrypt(userDto.Id.ToString())),
            new(ClaimTypes.Name, userDto.Username),
            new(ClaimTypes.Expiration, expirationDate.ToString(CultureInfo.InvariantCulture))
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configSettings.AuthSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationDate,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}