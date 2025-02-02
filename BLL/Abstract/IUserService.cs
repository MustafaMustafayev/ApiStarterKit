using DAL.EntityFramework.Utility;
using DTO;
using DTO.Responses;
using DTO.User;

namespace BLL.Abstract;

public interface IUserService
{
    Task<IDataResult<PaginatedList<UserResponseDto>>> GetAsGenericListAsync(GenericRequestDto dto);
    Task<IDataResult<IEnumerable<UserResponseDto>>> GetAsync();
    Task<IDataResult<UserByIdResponseDto>> GetAsync(Guid id);

    Task<IResult> AddAsync(UserCreateRequestDto dto);
    Task<IResult> UpdateAsync(Guid id, UserUpdateRequestDto dto);

    Task<IResult> SoftDeleteAsync(Guid id);
    Task<IResult?> SoftDeleteMultipleAsync(IList<Guid> ids);

    Task<IResult> DuplicateAsync(Guid userId);

    Task<IResult> SetImageAsync(Guid id, string? image = null);
    Task<IDataResult<string>> GetImageAsync(Guid id);
}