using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Identity.Services;

public class IdentityTokenService : IIdentityTokenService
{
    private string _cachedToken;
    private DateTime _tokenExpiration;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public IdentityTokenService(
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _cachedToken = string.Empty;
        _tokenExpiration = DateTime.MinValue;
    }

    public async Task<string> GetValidTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && !IsTokenExpired())
        {
            return _cachedToken;
        }

        var body = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _configuration["Keycloak:ClientId"] ?? throw new ArgumentNullException("Keycloak:ClientId") },
            { "client_secret", _configuration["Keycloak:ClientSecret"] ?? throw new ArgumentNullException("Keycloak:ClientSecret") }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/realms/rent-acme/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(body)
        };
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return _cachedToken;
        }

        var jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        _cachedToken = jsonResponse.RootElement.GetProperty("access_token").GetString() ?? "";
        _tokenExpiration = DateTime.UtcNow.AddSeconds(jsonResponse.RootElement.GetProperty("expires_in").GetInt32());

        return _cachedToken;
    }

    private bool IsTokenExpired()
    {
        return DateTime.UtcNow >= _tokenExpiration;
    }
}
