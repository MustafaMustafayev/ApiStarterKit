using AutoMapper;
using DTO.Log;
using ENTITIES.Entities;

namespace BLL.Mappers;

public class LogMapper : Profile
{
    public LogMapper()
    {
        CreateMap<Log, LogResponseDto>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime.ToString("dd/MM/yyyy HH:mm")));

        CreateMap<LogCreateRequestDto, Log>();
    }
}