using API.Attributes;
using API.Filters;
using BLL.Abstract;
using CORE.Abstract;
using CORE.Constants;
using CORE.Enums;
using CORE.Helpers;
using CORE.Localization;
using DTO;
using DTO.Responses;
using DTO.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IResult = DTO.Responses.IResult;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateToken]
public class UsersController(
    IUserService userService,
    IUtilService utilService,
    ITokenResolverService tokenResolverService)
    : ControllerBase
{
    [SwaggerOperation(Summary = "list of users as paginated")]
    [Produces(typeof(IDataResult<List<UserResponseDto>>))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpPost("generic")]
    public async Task<IActionResult> GetAsPaginated([FromBody] GenericRequestDto dto)
    {
        string password = "12234";
        var response = await userService.GetAsGenericListAsync(dto);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "list of users")]
    [Produces(typeof(IDataResult<List<UserResponseDto>>))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await userService.GetAsync();
        return Ok(response);
    }

    [SwaggerOperation(Summary = "get user by id")]
    [Produces(typeof(IDataResult<UserResponseDto>))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await userService.GetAsync(id);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "create user")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserCreateRequestDto dto)
    {
        var response = await userService.AddAsync(dto);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "update user")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UserUpdateRequestDto dto)
    {
        var response = await userService.UpdateAsync(id, dto);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "delete user")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var response = await userService.SoftDeleteAsync(id);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "multi delete user")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpDelete("multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] IList<Guid> ids)
    {
        var response = await userService.SoftDeleteMultipleAsync(ids);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "duplicate user")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpGet("duplicate")]
    public async Task<IActionResult> Duplicate([FromQuery] Guid id)
    {
        var response = await userService.DuplicateAsync(id);
        return Ok(response);
    }

    [SwaggerOperation(Summary = "upload image")]
    [Produces(typeof(IResult))]
    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var userId = tokenResolverService.GetUserIdFromToken();

        var fileExtension = Path.GetExtension(file.FileName);
        var fileNewName = Guid.NewGuid();

        if (!Constants.AllowedImageExtensions.Contains(fileExtension))
        {
            return BadRequest(new ErrorDataResult<string>(EMessages.ThisFileTypeIsNotAllowed.Translate()));
        }

        var path = utilService.GetEnvFolderPath(utilService.GetFolderName(EFileType.UserImage));
        await FileHelper.WriteFile(file, $"{fileNewName}{fileExtension}", path);

        await userService.SetImageAsync(userId!.Value, $"{fileNewName}{fileExtension}");

        return Ok(new SuccessResult(EMessages.Success.Translate()));
    }

    [SwaggerOperation(Summary = "delete image")]
    [Produces(typeof(IResult))]
    [ServiceFilter(typeof(LogActionFilter))]
    [HttpDelete("image")]
    public async Task<IActionResult> DeleteImage()
    {
        var userId = tokenResolverService.GetUserIdFromToken();

        var existFile = await userService.GetImageAsync(userId!.Value);

        if (!existFile.Success)
        {
            return BadRequest(existFile);
        }

        if (existFile.Data is null)
        {
            return Ok(new SuccessResult(EMessages.Success.Translate()));
        }

        var path = utilService.GetEnvFolderPath(utilService.GetFolderName(EFileType.UserImage));
        var fullPath = Path.Combine(path, existFile.Data);

        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }

        await userService.SetImageAsync(userId.Value);

        return Ok(new SuccessResult(EMessages.Success.Translate()));
    }
}