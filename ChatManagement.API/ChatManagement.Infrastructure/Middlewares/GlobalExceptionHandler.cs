using System.Text.Json;
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
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var responseDto = new ResponseDto
            {
                IsSuccess = false,
                Message = ex.Message
            };
            var jsonResponse = JsonSerializer.Serialize(responseDto);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}