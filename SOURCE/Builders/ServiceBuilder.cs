using SOURCE.Builders.Abstract;
using SOURCE.Helpers;
using SOURCE.Models;
using SOURCE.Workers;

namespace SOURCE.Builders;

// ReSharper disable once UnusedType.Global
public class ServiceBuilder : ISourceBuilder
{
    public void BuildSourceFile(List<Entity> entities)
    {
        entities
            .Where(w =>
                w.Options.BuildService
                && w.Options.BuildDto
                && w.Options.BuildRepository)
            .ToList().ForEach(model =>
            SourceBuilder.Instance.AddSourceFile(Constants.SERVICE_PATH, $"{model.Name}Service.cs",
                BuildSourceText(model, null)));
    }

    public string BuildSourceText(Entity? entity, List<Entity>? entities)
    {
        var text = """
                   using AutoMapper;
                   using BLL.Abstract;
                   using CORE.Localization;
                   using DAL.EntityFramework.Abstract;
                   using DAL.EntityFramework.Utility;
                   using DTO.Responses;
                   using DTO;
                   using DTO.{entityName};
                   using ENTITIES.Entities{entityPath};
                   using DAL.EntityFramework.UnitOfWork;
                   
                   namespace BLL.Concrete;

                   public class {entityName}Service(IMapper mapper,
                                                    IUnitOfWork unitOfWork,
                                                    I{entityName}Repository {entityNameLower}Repository) : I{entityName}Service
                   {                   
                       public async Task<IResult> AddAsync({entityName}CreateRequestDto dto)
                       {
                           var data = mapper.Map<{entityName}>(dto);       
                           await {entityNameLower}Repository.AddAsync(data);
                           await unitOfWork.CommitAsync();
                   
                           return new SuccessResult(EMessages.Success.Translate());
                       }
                   
                       public async Task<IResult> SoftDeleteAsync(Guid id)
                       {
                           var data = await {entityNameLower}Repository.GetAsync(m => m.Id == id);
                           if (data is null)
                           {
                               return new ErrorResult(EMessages.DataNotFound.Translate());
                           }
                           {entityNameLower}Repository.SoftDelete(data);
                           await unitOfWork.CommitAsync();

                           return new SuccessResult(EMessages.Success.Translate());
                       }
                   
                       public async Task<IDataResult<PaginatedList<{entityName}ResponseDto>>> GetAsGenericListAsync(GenericRequestDto dto)
                       {
                           var datas = await {entityNameLower}Repository.GetAsGenericListAsync(dto);
                       
                           var responseDto = new PaginatedList<{entityName}ResponseDto>(mapper.Map<List<{entityName}ResponseDto>>(datas.Datas),
                                                                                datas.TotalRecordCount,
                                                                                datas.PageIndex,
                                                                                dto.Pagination?.PageSize ?? 0);
                       
                           return new SuccessDataResult<PaginatedList<{entityName}ResponseDto>>(responseDto, EMessages.Success.Translate());
                       }
                   
                       public async Task<IDataResult<IEnumerable<{entityName}ResponseDto>>> GetAsync()
                       {
                           var datas = mapper.Map<IEnumerable<{entityName}ResponseDto>>(await {entityNameLower}Repository.GetListAsync());                  
                           return new SuccessDataResult<IEnumerable<{entityName}ResponseDto>>(datas, EMessages.Success.Translate());
                       }
                   
                       public async Task<IDataResult<{entityName}ByIdResponseDto>> GetAsync(Guid id)
                       {
                           var data = mapper.Map<{entityName}ByIdResponseDto>(await {entityNameLower}Repository.GetAsync(m => m.Id == id));              
                           return new SuccessDataResult<{entityName}ByIdResponseDto>(data, EMessages.Success.Translate());
                       }
                   
                       public async Task<IResult> UpdateAsync(Guid id, {entityName}UpdateRequestDto dto)
                       {
                           var data = mapper.Map<{entityName}>(dto);
                           data.Id = id;              
                           {entityNameLower}Repository.Update(data);
                           await unitOfWork.CommitAsync();
                   
                           return new SuccessResult(EMessages.Success.Translate());
                       }
                   }
                   """;

        text = text.Replace("{entityName}", entity!.Name);
        text = text.Replace("{entityNameLower}", entity.Name.FirstCharToLowerCase());
        text = text.Replace("{entityPath}", !string.IsNullOrEmpty(entity!.Path) ? $".{entity.Path}" : string.Empty);
        return text;
    }
}