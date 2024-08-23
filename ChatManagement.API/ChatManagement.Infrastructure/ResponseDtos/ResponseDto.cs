using System.Text.Json.Serialization;

namespace ChatManagement.Infrastructure.ResponseDtos;

public class ResponseDto
{
    //[JsonPropertyName("result")]
    public object? Result { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? Message { get; set; } = string.Empty;
}