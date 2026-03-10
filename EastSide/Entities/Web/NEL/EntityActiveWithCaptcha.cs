using System.Text.Json.Serialization;

namespace EastSide.Entities.Web.NEL;

public class EntityActiveWithCaptcha
{
	[JsonPropertyName("id")]
	public required string UserId { get; set; }

	[JsonPropertyName("identifier")]
	public required string Identifier { get; set; }

	[JsonPropertyName("captcha")]
	public required string Captcha { get; set; }
}
