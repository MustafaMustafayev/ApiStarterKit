using DTO.Auth;
using DTO.Responses;
using DTO.Token;

namespace BLL.Abstract;

public interface ITokenService
{
    Task<IResult> AddAsync(LoginResponseDto dto);
    Task<IResult> SoftDeleteAsync(Guid id);
    Task<IDataResult<TokenToListDto>> GetAsync(string accessToken, string refreshToken);
    Task<IResult> CheckValidationAsync(string accessToken);
    Task<IDataResult<LoginResponseDto>> CreateTokenAsync(Guid userId);
}