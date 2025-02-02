using DAL.EntityFramework.Utility;
using DTO;
using DTO.Log;
using DTO.Responses;

namespace BLL.Abstract;

public interface ILogService
{
    Task<IDataResult<PaginatedList<LogResponseDto>>> GetAsGenericListAsync(GenericRequestDto dto);
    Task<IDataResult<IEnumerable<LogResponseDto>>> GetAsync();
    Task<IDataResult<LogResponseDto>> GetAsync(Guid id);

    Task<IResult> AddAsync(LogCreateRequestDto requestDto);
}