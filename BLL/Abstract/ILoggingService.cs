using DTO.Logging;

namespace BLL.Abstract;

public interface ILoggingService
{
    Task AddAsync(RequestLogDto dto);
}