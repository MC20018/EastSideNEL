using System;
using System.Text.Json.Serialization;
using Codexus.Cipher.Entities.Converter;

namespace EastSide.Core.Entity;

public class Entities<T> : EntityResponse
{
	[JsonPropertyName("details")]
	public string Details { get; set; } = string.Empty;

	[JsonPropertyName("entities")]
	public T[] Data { get; set; } = Array.Empty<T>();

	[JsonPropertyName("total")]
	[JsonConverter(typeof(NetEaseStringConverter))]
	public int Total { get; set; }
}
