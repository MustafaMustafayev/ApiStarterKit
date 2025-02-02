using API.Attributes;
using API.Filters;
using BLL.Abstract;
using CORE.Abstract;
using CORE.Helpers;
using CORE.Localization;
using DTO.Auth;
using DTO.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IResult = DTO.Responses.IResult;
using Result = DTO.Responses.Result;

namespace API.Controllers;

[Route("api/[controller]")]
[ServiceFilter(typeof(LogActionFilter))]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AuthController(
    IAuthService authService,
    ITokenResolverService tokenResolverService,
    ITokenService tokenService)
    : ControllerBase
{
    [SwaggerOperation(Summary = "login")]
    [Produces(typeof(IDataResult<LoginResponseDto>))]
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var userSalt = await authService.GetUserSaltAsync(request.Email);

        if (string.IsNullOrEmpty(userSalt))
        {
            return Ok(new ErrorDataResult<Result>(EMessages.InvalidUserCredentials.Translate()));
        }
        request = request with { Password = SecurityHelper.HashPassword(request.Password, userSalt) };

        var loginResult = await authService.LoginAsync(request);
        if (!loginResult.Success)
        {
            return Unauthorized(loginResult);
        }

        var response = await tokenService.CreateTokenAsync(loginResult.Data!.Id);

        return Ok(response);
    }

    [SwaggerOperation(Summary = "refresh access token")]
    [Produces(typeof(IDataResult<LoginResponseDto>))]
    [ValidateToken]
    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var jwtToken = tokenResolverService.GetAccessToken();
        var refreshToken = tokenResolverService.GetRefreshToken();

        var userId = tokenResolverService.GetUserIdFromToken();

        var tokenResponse = await tokenService.GetAsync(jwtToken!, refreshToken!);
        if (!tokenResponse.Success)
        {
            return Unauthorized(tokenResponse);
        }

        await tokenService.SoftDeleteAsync(tokenResponse.Data!.Id);
        var response = await tokenService.CreateTokenAsync(userId!.Value);

        return Ok(response);
    }

    [SwaggerOperation(Summary = "reset password")]
    [Produces(typeof(IResult))]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var userId = tokenResolverService.GetUserIdFromToken();
        if (userId is null)
        {
            return Unauthorized(new ErrorResult(EMessages.PermissionDenied.Translate()));
        }

        var response = await authService.ResetPasswordAsync(userId.Value, request);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "login by token")]
    [Produces(typeof(IDataResult<LoginResponseDto>))]
    [ValidateToken]
    [HttpGet("login/token")]
    public async Task<IActionResult> LoginByToken()
    {
        if (string.IsNullOrEmpty(tokenResolverService.GetAccessToken()))
        {
            return Unauthorized(new ErrorResult(EMessages.PermissionDenied.Translate()));
        }

        var loginByTokenResponse = await authService.LoginByTokenAsync();
        if (!loginByTokenResponse.Success)
        {
            return BadRequest(loginByTokenResponse.Data);
        }

        return Ok(loginByTokenResponse);
    }

    [SwaggerOperation(Summary = "logout")]
    [Produces(typeof(IResult))]
    [HttpGet("logout")]
    [ValidateToken]
    public async Task<IActionResult> Logout()
    {
        var userId = tokenResolverService.GetUserIdFromToken();
        if (userId is null)
        {
            return Unauthorized(new ErrorResult(EMessages.PermissionDenied.Translate()));
        }

        var response = await authService.LogoutAsync(userId.Value);

        return Ok(response);
    }
}