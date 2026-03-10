using System.Text.Json.Serialization;

namespace EastSide.Core.Entity;

public class Entity<T> : EntityResponse
{
	[JsonPropertyName("details")]
	public string Details { get; set; } = string.Empty;

	[JsonPropertyName("entity")]
	public T? Data { get; set; }
}
