using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Razor.Admin.Services;

public record AuthTokenResponse(
    string accessToken,
    string tokenType,
    int expiresInSeconds
);

public class AuthClientAdmin
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public AuthClientAdmin(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<(bool ok, string? error, AuthTokenResponse? token)>
        LoginAsync(string email, string password)
    {
        var baseUrl = _config["Gateway:BaseUrl"] ?? "https://localhost:7000";

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        var payload = new { Email = email, Password = password };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            return (false, $"Login failed: {text}", null);
        }

        var token = await response.Content.ReadFromJsonAsync<AuthTokenResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return (true, null, token);
    }
}
