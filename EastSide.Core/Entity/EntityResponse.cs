using System.Text.Json.Serialization;

namespace EastSide.Core.Entity;

public class EntityResponse
{
	[JsonPropertyName("code")]
	public int Code { get; set; }

	[JsonPropertyName("message")]
	public string Message { get; set; } = string.Empty;
}

public class EntityResponse<T> : EntityResponse
{
	[JsonPropertyName("data")]
	public T? Data { get; set; }
}
