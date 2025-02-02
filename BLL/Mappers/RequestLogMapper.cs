using AutoMapper;
using DTO.Logging;
using ENTITIES.Entities;

namespace BLL.Mappers;

public class RequestLogMapper : Profile
{
    public RequestLogMapper()
    {
        CreateMap<RequestLogDto, RequestLog>();
    }
}