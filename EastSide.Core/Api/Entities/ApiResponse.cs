using System.Text.Json.Serialization;

namespace EastSide.Core.Api.Entities;

public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public static ApiResponse Fail(string message) => new() { Success = false, Message = message };
    public static ApiResponse Ok(string? message = null) => new() { Success = true, Message = message };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public new static ApiResponse<T> Fail(string message) => new() { Success = false, Message = message };
    public static ApiResponse<T> Ok(T data, string? message = null) => new() { Success = true, Data = data, Message = message };
}
