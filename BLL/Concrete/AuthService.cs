using AutoMapper;
using BLL.Abstract;
using CORE.Abstract;
using CORE.Helpers;
using CORE.Localization;
using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.UnitOfWork;
using DTO.Auth;
using DTO.Responses;
using DTO.User;

namespace BLL.Concrete;

public class AuthService(
    IMapper mapper,
    ITokenResolverService tokenResolverService,
    IUserRepository userRepository,
    ITokenRepository tokenRepository,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<string?> GetUserSaltAsync(string userEmail)
    {
        return await userRepository.GetUserSaltAsync(userEmail);
    }

    public async Task<IDataResult<UserResponseDto>> LoginAsync(LoginRequestDto dtos)
    {
        var data = await userRepository.GetAsync(m =>
            m.Email == dtos.Email && m.Password == dtos.Password);

        if (data == null)
        {
            return new ErrorDataResult<UserResponseDto>(EMessages.InvalidUserCredentials.Translate());
        }

        var mapped = mapper.Map<UserResponseDto>(data);

        return new SuccessDataResult<UserResponseDto>(mapped,
                                                      EMessages.Success.Translate());
    }

    public async Task<IDataResult<UserResponseDto>> LoginByTokenAsync()
    {
        var userId = tokenResolverService.GetUserIdFromToken();
        if (userId is null)
        {
            return new ErrorDataResult<UserResponseDto>(EMessages.PermissionDenied.Translate());
        }

        var data = await userRepository.GetAsync(m => m.Id == userId);
        if (data == null)
        {
            return new ErrorDataResult<UserResponseDto>(EMessages.InvalidUserCredentials.Translate());
        }

        return new SuccessDataResult<UserResponseDto>(mapper.Map<UserResponseDto>(data),
                                                      EMessages.Success.Translate());
    }

    public async Task<IResult> ResetPasswordAsync(Guid userId, ResetPasswordRequestDto dto)
    {
        var data = await userRepository.GetAsync(m => m.Id == userId);

        if (data is null)
        {
            return new ErrorResult(EMessages.UserIsNotExist.Translate());
        }

        if (data.LastVerificationCode is null ||
            !data.LastVerificationCode.Equals(dto.VerificationCode))
        {
            return new ErrorResult(EMessages.PermissionDenied.Translate());
        }

        data.Salt = SecurityHelper.GenerateSalt();
        data.Password = SecurityHelper.HashPassword(dto.Password, data.Salt);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IResult> LogoutAsync(Guid userId)
    {
        await tokenRepository.RemoveUserActiveTokensAsync(userId);
        return new SuccessResult(EMessages.Success.Translate());
    }
}