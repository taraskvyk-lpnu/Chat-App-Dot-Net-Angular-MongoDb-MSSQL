using System.Text.Json;
using ChatManagement.Infrastructure.CustomException;
using ChatManagement.Infrastructure.ResponseDtos;
using Microsoft.AspNetCore.Http;

namespace ChatManagement.Infrastructure.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException notFoundEx)
        {
            context.Response.StatusCode = 404;
            await WriteResponse(context, notFoundEx.Message, 404);
        }
        catch (AccessViolationException accessEx)
        {
            context.Response.StatusCode = 403;
            await WriteResponse(context, accessEx.Message, 403);
        }
        catch (UserAttachmentException userAttachEx)
        {
            context.Response.StatusCode = 409;
            await WriteResponse(context, userAttachEx.Message, 409);
        }
        catch (ApiException apiEx)
        {
            context.Response.StatusCode = apiEx.StatusCode;
            await WriteResponse(context, apiEx.Message, apiEx.StatusCode);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await WriteResponse(context, "An unexpected error has occurred.", 500);
        }
    }

    private static async Task WriteResponse(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        var responseDto = new ResponseDto
        {
            IsSuccess = false,
            Message = $"{statusCode}: " + message
        };
        var jsonResponse = JsonSerializer.Serialize(responseDto);
        await context.Response.WriteAsync(jsonResponse);
    }
}