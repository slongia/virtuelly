using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Razor.Admin.Services;

// Must match IdentityService DTO contracts

public record AdminUserListItemDto(
    string Id,
    string Email,
    bool EmailConfirmed,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    int AccessFailedCount,
    string[] Roles
);

public record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
);

public class AdminUsersClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminUsersClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<AdminUserListItemDto>> GetUsersAsync(
        int pageNumber,
        int pageSize,
        string? sortBy,
        string? sortDir,
        string? search)
    {
        var client = CreateGatewayClientWithBearer();

        // Build query string
        var qs = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };

        if (!string.IsNullOrWhiteSpace(sortBy))
            qs.Add($"sortBy={Uri.EscapeDataString(sortBy)}");

        if (!string.IsNullOrWhiteSpace(sortDir))
            qs.Add($"sortDir={Uri.EscapeDataString(sortDir)}");

        if (!string.IsNullOrWhiteSpace(search))
            qs.Add($"search={Uri.EscapeDataString(search)}");

        var url = "/api/users";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to load users. Status {response.StatusCode}. Body: {text}");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<AdminUserListItemDto>>(options);

        if (payload == null)
        {
            return new PagedResult<AdminUserListItemDto>(
                Items: Array.Empty<AdminUserListItemDto>(),
                PageNumber: pageNumber,
                PageSize: pageSize,
                TotalCount: 0
            );
        }

        return payload;
    }

    private HttpClient CreateGatewayClientWithBearer()
    {
        var baseUrl = _config["Gateway:BaseUrl"] ?? "https://localhost:7000";
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        // pull the JWT we stored at login/registration
        var user = _httpContextAccessor.HttpContext?.User;
        var token = user?.Claims.FirstOrDefault(c => c.Type == "accessToken")?.Value;

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
}
