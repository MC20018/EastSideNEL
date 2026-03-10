using System.Text.Json.Serialization;

namespace EastSide.Entities.Web.NEL;

public class EntitySecurityVerify
{
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("verify_url")]
    public string VerifyUrl { get; set; } = string.Empty;

    public bool IsSecurityVerify => Code == 1351;
}
