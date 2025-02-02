using BLL.Abstract;
using CORE.Abstract;
using CORE.Config;
using CORE.Localization;
using DTO.Log;
using DTO.Responses;
using ENTITIES.Enums;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using System.Text.Json;

namespace API.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    ConfigSettings config,
    IServiceScopeFactory serviceScopeFactory)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError("Something went wrong: {Exception}", ex);
            await LogErrorAsync(httpContext, ex);
            await HandleExceptionAsync(httpContext);
        }
    }

    private async Task LogErrorAsync(HttpContext httpContext, Exception ex)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var tokenResolverService = scope.ServiceProvider.GetRequiredService<ITokenResolverService>();
        var logService = scope.ServiceProvider.GetRequiredService<ILogService>();

        var traceIdentifier = httpContext.TraceIdentifier;
        var clientIp = httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
        var path = httpContext.Request.Path;
        var stackTrace = ex.StackTrace?.Length > 2000 ? ex.StackTrace[..2000] : ex.StackTrace;
        var token = string.Empty;
        Guid? userId = null;
        var authHeaderName = config.AuthSettings.HeaderName;

        if (!string.IsNullOrEmpty(httpContext.Request.Headers[authHeaderName]) &&
            httpContext.Request.Headers[authHeaderName].ToString().Length > 7)
        {
            token = httpContext.Request.Headers[authHeaderName].ToString();
            userId = !string.IsNullOrEmpty(token) ? tokenResolverService.GetUserIdFromToken() : null;
        }

        LogCreateRequestDto logToAddRequestDto = new()
        {
            AccessToken = token,
            UserId = userId,
            Path = path,
            Ip = clientIp,
            Text = ex.Message,
            Type = ELogType.Error,
            StackTrace = stackTrace
        };
        await logService.AddAsync(logToAddRequestDto);
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new ErrorResult(EMessages.GeneralError.Translate());
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}