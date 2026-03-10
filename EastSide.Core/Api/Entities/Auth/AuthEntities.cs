using System.Text.Json.Serialization;
using EastSide.Core.Api.Entities;

namespace EastSide.Core.Api.Entities.Auth;

public class LoginResponse : ApiResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }
}

public class TokenAuthUser
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("rank")]
    public string? Rank { get; set; }

    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }
}

public class TokenAuthResponse : ApiResponse
{
    [JsonPropertyName("user")]
    public TokenAuthUser? User { get; set; }
}

public class UserInfoResponse : ApiResponse
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("rank")]
    public string? Rank { get; set; }

    [JsonPropertyName("banned")]
    public int Banned { get; set; }

    [JsonPropertyName("isAdmin")]
    public int IsAdmin { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("lastLogin")]
    public string? LastLogin { get; set; }
}

public class DurationResponse : ApiResponse
{
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }
}

public class CardKeyActivateResponse : ApiResponse
{
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }
}

public class UserUrlResponse : ApiResponse
{
    [JsonPropertyName("userUrl")]
    public string? UserUrl { get; set; }
}

public class CrcSaltResponse : ApiResponse
{
    [JsonPropertyName("salt")]
    public string? Salt { get; set; }

    [JsonPropertyName("gameVersion")]
    public string? GameVersion { get; set; }

    [JsonPropertyName("id")]
    public long? Id { get; set; }
}

public class GenerateCardKeyResponse : ApiResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("cardKeys")]
    public CardKeyItem[]? CardKeys { get; set; }
}

public class CardKeyItem
{
    [JsonPropertyName("cardKey")]
    public string CardKey { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public long Duration { get; set; }

    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }
}

public class AdminUserSearchResponse : ApiResponse
{
    [JsonPropertyName("user")]
    public AdminUserInfo? User { get; set; }
}

public class AdminUserListResponse : ApiResponse
{
    [JsonPropertyName("users")]
    public List<AdminUserInfo> Users { get; set; } = new();

    [JsonPropertyName("totalElements")]
    public long TotalElements { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }
}

public class AdminUserInfo
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("rank")]
    public string Rank { get; set; } = string.Empty;

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("duration")]
    public long Duration { get; set; }

    [JsonPropertyName("durationText")]
    public string DurationText { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("lastLogin")]
    public string LastLogin { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusText")]
    public string StatusText { get; set; } = string.Empty;
}

public class DeriveUuidResponse : ApiResponse
{
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
}
