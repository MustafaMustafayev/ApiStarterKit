using AutoMapper;
using BLL.Abstract;
using CORE.Localization;
using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Concrete;
using DAL.EntityFramework.UnitOfWork;
using DAL.EntityFramework.Utility;
using DTO;
using DTO.Log;
using DTO.Responses;
using DTO.User;
using ENTITIES.Entities;

namespace BLL.Concrete;

public class LogService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogRepository logRepository) : ILogService
{
    public async Task<IResult> AddAsync(LogCreateRequestDto requestDto)
    {
        var data = mapper.Map<Log>(requestDto);
        await logRepository.AddAsync(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IDataResult<PaginatedList<LogResponseDto>>> GetAsGenericListAsync(GenericRequestDto dto)
    {
        var datas = await logRepository.GetAsGenericListAsync(dto);

        var responseDto = new PaginatedList<LogResponseDto>(mapper.Map<List<LogResponseDto>>(datas.Datas),
                                                             datas.TotalRecordCount,
                                                             datas.PageIndex,
                                                             dto.Pagination?.PageSize ?? 0);

        return new SuccessDataResult<PaginatedList<LogResponseDto>>(responseDto, EMessages.Success.Translate());
    }

    public async Task<IDataResult<IEnumerable<LogResponseDto>>> GetAsync()
    {
        var datas = mapper.Map<IEnumerable<LogResponseDto>>(await logRepository.GetListAsync());

        return new SuccessDataResult<IEnumerable<LogResponseDto>>(datas, EMessages.Success.Translate());
    }

    public async Task<IDataResult<LogResponseDto>> GetAsync(Guid id)
    {
        var data = mapper.Map<LogResponseDto>(await logRepository.GetAsync(m => m.Id == id));

        return new SuccessDataResult<LogResponseDto>(data, EMessages.Success.Translate());
    }
}