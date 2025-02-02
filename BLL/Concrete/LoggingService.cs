using AutoMapper;
using BLL.Abstract;
using DAL.EntityFramework.Abstract;
using DTO.Logging;
using ENTITIES.Entities;

namespace BLL.Concrete;

public class LoggingService(
    IMapper mapper,
    IRequestLogRepository requestLogRepository) : ILoggingService
{
    public async Task AddAsync(RequestLogDto dto)
    {
        var data = mapper.Map<RequestLog>(dto);
        await requestLogRepository.AddAsync(data);
    }
}