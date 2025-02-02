using API.Attributes;
using API.Filters;
using BLL.Abstract;
using DTO;
using DTO.Log;
using DTO.Responses;
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
public class LogsController(ILogService logService) : ControllerBase
{
    [SwaggerOperation(Summary = "list of logs as paginated")]
    [Produces(typeof(IDataResult<List<LogResponseDto>>))]
    [HttpPost("generic")]
    public async Task<IActionResult> GetAsGeneric([FromBody] GenericRequestDto dto)
    {
        var response = await logService.GetAsGenericListAsync(dto);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "list of logs")]
    [Produces(typeof(IDataResult<List<LogResponseDto>>))]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await logService.GetAsync();
        return Ok(response);
    }

    [SwaggerOperation(Summary = "get log by id")]
    [Produces(typeof(IDataResult<LogResponseDto>))]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await logService.GetAsync(id);
        return Ok(response);
    }
}