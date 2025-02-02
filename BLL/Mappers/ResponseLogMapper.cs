using AutoMapper;
using DTO.Logging;
using ENTITIES.Entities;

namespace BLL.Mappers;

public class ResponseLogMapper : Profile
{
    public ResponseLogMapper()
    {
        CreateMap<ResponseLogDto, ResponseLog>();
    }
}