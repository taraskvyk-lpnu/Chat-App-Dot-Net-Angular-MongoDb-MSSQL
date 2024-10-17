using System.Text.Json.Serialization;

namespace ChatManagement.Infrastructure.ResponseDtos;

public class ResponseDto
{
    public bool IsSuccess { get; set; } = true;
    public object Data { get; set; } = new object();
    public string? Message { get; set; } = string.Empty;
}