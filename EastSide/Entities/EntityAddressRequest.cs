using System.Text.Json.Serialization;

namespace EastSide.Entities;

public class EntityAddressRequest
{
    [JsonPropertyName("item_id")] public string ItemId { get; set; } = string.Empty;
}
