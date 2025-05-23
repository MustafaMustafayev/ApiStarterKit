﻿using BLL.Abstract;
using CORE.Abstract;
using CORE.Config;
using DTO.Logging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class LogActionFilter(
    ITokenResolverService tokenResolverService,
    ILoggingService loggingService,
    ConfigSettings configSettings)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var traceIdentifier = httpContext.TraceIdentifier;
        var request = httpContext.Request;

        var clientIp = httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString();
        var uri = request.Host + request.Path + request.QueryString;

        var token = string.Empty;
        Guid? userId = null;
        var authHeaderName = configSettings.AuthSettings.HeaderName;

        if (!string.IsNullOrEmpty(request.Headers[authHeaderName]) &&
            request.Headers[authHeaderName].ToString().Length > 7)
        {
            token = request.Headers[authHeaderName].ToString();
            userId = !string.IsNullOrEmpty(token) ? tokenResolverService.GetUserIdFromToken() : null;
        }

        request.Body.Position = 0;
        using var streamReader = new StreamReader(request.Body);
        var bodyContent = await streamReader.ReadToEndAsync();
        request.Body.Position = 0;

        await next();

        var requestLog = new RequestLogDto
        {
            TraceIdentifier = traceIdentifier,
            ClientIp = clientIp!,
            Uri = uri,
            RequestDate = DateTime.Now,
            Payload = bodyContent,
            Method = request.Method,
            Token = token,
            UserId = userId,
            ResponseLog = new ResponseLogDto
            {
                TraceIdentifier = traceIdentifier,
                ResponseDate = DateTime.Now,
                StatusCode = httpContext.Response.StatusCode.ToString(),
                Token = token,
                UserId = userId
            }
        };

        await loggingService.AddAsync(requestLog);
    }
}