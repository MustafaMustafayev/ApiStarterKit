using AutoMapper;
using BLL.Abstract;
using CORE.Abstract;
using CORE.Config;
using CORE.Helpers;
using CORE.Localization;
using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.UnitOfWork;
using DTO.Auth;
using DTO.Responses;
using DTO.Token;
using DTO.User;
using ENTITIES.Entities;

namespace BLL.Concrete;

public class TokenService(
    ConfigSettings configSettings,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ITokenRepository tokenRepository,
    IUserRepository userRepository,
    ITokenResolverService tokenResolverService,
    IEncryptionService encryptionService) : ITokenService
{
    public async Task<IResult> AddAsync(LoginResponseDto dto)
    {
        var data = mapper.Map<Token>(dto);
        await tokenRepository.AddAsync(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IDataResult<TokenToListDto>> GetAsync(string accessToken, string refreshToken)
    {
        var token = await tokenRepository.GetAsync(m => m.AccessToken == accessToken &&
                                                        m.RefreshToken == refreshToken &&
                                                        m.RefreshTokenExpireDate > DateTime.UtcNow);

        if (token == null)
        {
            return new ErrorDataResult<TokenToListDto>(EMessages.PermissionDenied.Translate());
        }

        var data = mapper.Map<TokenToListDto>(token);

        return new SuccessDataResult<TokenToListDto>(data, EMessages.Success.Translate());
    }

    public async Task<IResult> CheckValidationAsync(string accessToken)
    {
        return await tokenRepository.IsValidAsync(accessToken)
            ? new SuccessResult(EMessages.Success.Translate())
            : new ErrorResult(EMessages.PermissionDenied.Translate());
    }

    public async Task<IDataResult<LoginResponseDto>> CreateTokenAsync(Guid userId)
    {
        var securityHelper = new SecurityHelper(configSettings, encryptionService);
        var accessTokenExpireDate = DateTime.UtcNow.AddHours(configSettings.AuthSettings.TokenExpirationTimeInHours);

        var user = await userRepository.SingleOrDefaultAsync(m => m.Id == userId);
        var userDto = mapper.Map<UserResponseDto>(user);

        var loginResponseDto = new LoginResponseDto
        {
            User = userDto,
            AccessToken = securityHelper.CreateTokenForUser(userDto, accessTokenExpireDate),
            AccessTokenExpireDate = accessTokenExpireDate,
            RefreshToken = tokenResolverService.GenerateRefreshToken(),
            RefreshTokenExpireDate =
                accessTokenExpireDate.AddMinutes(configSettings.AuthSettings.RefreshTokenAdditionalMinutes)
        };

        await AddAsync(loginResponseDto);

        await unitOfWork.CommitAsync();

        return new SuccessDataResult<LoginResponseDto>(loginResponseDto, EMessages.Success.Translate());
    }

    public async Task<IResult> SoftDeleteAsync(Guid id)
    {
        var data = await tokenRepository.GetAsync(id);
        tokenRepository.SoftDelete(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }
}