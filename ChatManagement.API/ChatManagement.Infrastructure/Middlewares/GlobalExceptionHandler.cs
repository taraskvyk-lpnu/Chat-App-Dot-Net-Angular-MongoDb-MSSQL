using System.Text.Json;
using ChatManagement.Infrastructure.CustomException;
using ChatManagement.Infrastructure.ResponseDtos;
using Microsoft.AspNetCore.Http;

namespace ChatManagement.Infrastructure.Middlewares;

public class GlobalExceptionHandler
{
    private RequestDelegate _next;
    
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
        catch (ApiException apiEx)
        {
            context.Response.StatusCode = apiEx.StatusCode;
            await WriteResponse(context, apiEx.Message);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await WriteResponse(context, "An unexpected error has occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        var responseDto = new ResponseDto
        {
            IsSuccess = false,
            Message = message
        };
        var jsonResponse = JsonSerializer.Serialize(responseDto);
        await context.Response.WriteAsync(jsonResponse);
    }
}