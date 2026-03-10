using System.Text.Json.Serialization;
using EastSide.Core.Api.Entities;

namespace EastSide.Core.Api.Entities.System;

public class PluginInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("logoUrl")]
    public string? LogoUrl { get; set; }

    [JsonPropertyName("shortDescription")]
    public string? ShortDescription { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }

    [JsonPropertyName("depends")]
    public string? Depends { get; set; }
}

public class PluginListResponse : ApiResponse
{
    [JsonPropertyName("plugins")]
    public List<PluginInfo>? Plugins { get; set; }
}

public class PluginListWithItems : ApiResponse
{
    [JsonPropertyName("items")]
    public List<PluginInfo>? Items { get; set; }
}

public class AnnouncementResponse : ApiResponse
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("updated")]
    public string? Updated { get; set; }
}

public class VersionResponse : ApiResponse
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }
}

public class MemoryInfo
{
    [JsonPropertyName("total")]
    public long Total { get; set; }

    [JsonPropertyName("free")]
    public long Free { get; set; }

    [JsonPropertyName("used")]
    public long Used { get; set; }
}

public class ServerStatusResponse : ApiResponse
{
    [JsonPropertyName("uptime")]
    public long Uptime { get; set; }

    [JsonPropertyName("totalUsers")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("cpu")]
    public string? Cpu { get; set; }

    [JsonPropertyName("memory")]
    public MemoryInfo? Memory { get; set; }
}

public class CaptchaResponse
{
    [JsonPropertyName("result")]
    public string? Result { get; set; }
}

public class SmResultResponse : ApiResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("details")]
    public object? Details { get; set; }
}

public class UpdateRankResponse : ApiResponse
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("rank")]
    public string? Rank { get; set; }
}

public class Md5MappingResponse : ApiResponse
{
    [JsonPropertyName("bootstrapMd5")]
    public string? BootstrapMd5 { get; set; }

    [JsonPropertyName("datFileMd5")]
    public string? DatFileMd5 { get; set; }
}
