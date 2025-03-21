﻿using DTO.Auth;
using DTO.Responses;
using DTO.User;

namespace BLL.Abstract;

public interface IAuthService
{
    Task<string?> GetUserSaltAsync(string email);
    Task<IDataResult<UserResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<IDataResult<UserResponseDto>> LoginByTokenAsync();
    Task<IResult> LogoutAsync(Guid userId);
    Task<IResult> ResetPasswordAsync(Guid userId, ResetPasswordRequestDto dto);
}