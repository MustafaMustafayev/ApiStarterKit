using SOURCE.Builders.Abstract;
using SOURCE.Helpers;
using SOURCE.Models;
using SOURCE.Workers;

namespace SOURCE.Builders;

// ReSharper disable once UnusedType.Global
public class ControllerBuilder : ISourceBuilder
{
    public void BuildSourceFile(List<Entity> entities)
    {
        entities
            .Where(w =>
                w.Options.BuildController
                && w.Options.BuildService
                //&& w.Options.BuildUnitOfWork
                && w.Options.BuildRepository
                && w.Options.BuildDto)
            .ToList()
            .ForEach(model => SourceBuilder
                .Instance.AddSourceFile(
                    Constants.CONTROLLER_PATH,
                    $"{model.Name}sController.cs",
                    BuildSourceText(model, null)));
    }

    public string BuildSourceText(Entity? entity, List<Entity>? entities)
    {
        var text = """
                   using API.Attributes;
                   using API.Filters;
                   using BLL.Abstract;
                   using DTO;                 
                   using DTO.Responses;
                   using DTO.{entityName};
                   using Microsoft.AspNetCore.Authentication.JwtBearer;
                   using Microsoft.AspNetCore.Authorization;
                   using Microsoft.AspNetCore.Mvc;
                   using Swashbuckle.AspNetCore.Annotations;
                   using IResult = DTO.Responses.IResult;

                   namespace API.Controllers;

                   [Route("api/[controller]")]
                   [ServiceFilter(typeof(LogActionFilter))]
                   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                   [ValidateToken]
                   public class {entityName}sController(I{entityName}Service {entityNameLower}Service) : ControllerBase
                   {                   
                       [SwaggerOperation(Summary = "paginated list")]
                       [Produces(typeof(IDataResult<List<{entityName}ResponseDto>>))]
                       [HttpPost("generic")]
                       public async Task<IActionResult> GetAsPaginated([FromBody] GenericRequestDto dto)
                       {
                           var response = await {entityNameLower}Service.GetAsGenericListAsync(dto);
                           return Ok(response);
                       }
                   
                       [SwaggerOperation(Summary = "get list")]
                       [Produces(typeof(IDataResult<List<{entityName}ResponseDto>>))]
                       [HttpGet]
                       public async Task<IActionResult> Get()
                       {
                           var response = await {entityNameLower}Service.GetAsync();
                           return Ok(response);
                       }
                   
                       [SwaggerOperation(Summary = "get data")]
                       [Produces(typeof(IDataResult<{entityName}ByIdResponseDto>))]   
                       [HttpGet("{id}")]
                       public async Task<IActionResult> Get([FromRoute] Guid id)
                       {
                           var response = await {entityNameLower}Service.GetAsync(id);
                           return Ok(response);
                       }
                   
                       [SwaggerOperation(Summary = "create")]
                       [Produces(typeof(IResult))]   
                       [HttpPost]
                       public async Task<IActionResult> Add([FromBody] {entityName}CreateRequestDto dto)
                       {
                           var response = await {entityNameLower}Service.AddAsync(dto);
                           return Ok(response);
                       }
                   
                       [SwaggerOperation(Summary = "update")]
                       [Produces(typeof(IResult))]   
                       [HttpPut("{id}")]
                       public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] {entityName}UpdateRequestDto dto)
                       {
                           var response = await {entityNameLower}Service.UpdateAsync(id, dto);
                           return Ok(response);
                       }
                   
                       [SwaggerOperation(Summary = "delete")]
                       [Produces(typeof(IResult))]         
                       [HttpDelete("{id}")]
                       public async Task<IActionResult> Delete([FromRoute] Guid id)
                       {
                           var response = await {entityNameLower}Service.SoftDeleteAsync(id);
                           return Ok(response);
                       }
                   }
                   """;

        text = text.Replace("{entityName}", entity!.Name);
        text = text.Replace("{entityNameLower}", entity.Name.FirstCharToLowerCase());
        return text;
    }
}