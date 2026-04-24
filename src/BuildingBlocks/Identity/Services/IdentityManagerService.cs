using Identity.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Identity.Services;

public class IdentityManagerService : IIdentityManagerService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IIdentityTokenService _tokenService;

    public IdentityManagerService(
        HttpClient httpClient,
        IConfiguration configuration,
        IIdentityTokenService identityTokenService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Keycloak:BaseUrl"] ?? throw new ArgumentNullException("Keycloak:BaseUrl"));
        _tokenService = identityTokenService;
    }

    public async Task<IdentityUser?> GetUserByIdAsync(string Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/rent-acme/users/{Id}");
        var token = await _tokenService.GetValidTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IdentityUser>(json);
    }

    public async Task<bool> DeleteUserByIdAsync(string Id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/admin/realms/rent-acme/users/{Id}");
        var token = await _tokenService.GetValidTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.SendAsync(request);

        return response.IsSuccessStatusCode;
    }
}
