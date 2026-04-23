using System.Text.Json.Serialization;

namespace EventBus.Events;

public record KeycloakEventIntegrationEvent
{
    [JsonPropertyName("@class")]
    public string? ClassName { get; set; } = string.Empty;
    [JsonPropertyName("time")]
    public long? Time { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; } = string.Empty;
    [JsonPropertyName("realmId")]
    public string? RealmId { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; } = string.Empty;
    [JsonPropertyName("userId")]
    public string? UserId { get; set; } = string.Empty;
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; } = string.Empty;
    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; set; } = string.Empty;
    [JsonPropertyName("error")]
    public string? Error { get; set; } = string.Empty;
    [JsonPropertyName("details")]
    public AuthDetails? AuthDetails { get; set; } = null!;
}

public record AuthDetails
{
    [JsonPropertyName("auth_method")]
    public string? AuthMethod { get; set; } = string.Empty;
    [JsonPropertyName("auth_type")]
    public string? AuthType { get; set; } = string.Empty;
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; } = string.Empty;
    [JsonPropertyName("code_id")]
    public string? CodeId { get; set; } = string.Empty;
    [JsonPropertyName("username")]
    public string? Username { get; set; } = string.Empty;
}