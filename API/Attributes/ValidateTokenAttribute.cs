using BLL.Abstract;
using CORE.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Attributes;

public class ValidateTokenAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (hasAllowAnonymous)
        {
            return;
        }

        var tokenService = (ITokenService)context.HttpContext.RequestServices.GetService(typeof(ITokenService))!;
        var tokenResolverService = (ITokenResolverService)context.HttpContext.RequestServices.GetService(typeof(ITokenResolverService))!;

        var jwtToken = tokenResolverService.GetAccessToken();

        var validationResult = tokenService.CheckValidationAsync(jwtToken).Result;

        if (!validationResult.Success)
        {
            context.Result = new UnauthorizedObjectResult(validationResult);
        }
    }
}