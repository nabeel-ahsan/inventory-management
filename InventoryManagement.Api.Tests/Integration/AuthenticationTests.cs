using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace InventoryManagement.Api.Tests.Integration;

public class AuthenticationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/product/addProduct", new { });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CustomerToken_WhenAccessingAdminEndpoint_ReturnsForbidden()
    {
        // Login as Customer
        var loginRequest = new { email = "admin", password = "admin" };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginBody.GetProperty("token").GetString();

        // Attach Customer JWT
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );

        // Try to access Admin-only endpoint
        var response = await _client.PostAsJsonAsync(
            "/api/product/addProduct",
            new
            {
                name = "Integration Test Product",
                description = "Test",
                price = 100,
                stockQuantity = 10,
                categoryId = Guid.NewGuid(),
            }
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminToken_WhenAccessingAdminEndpoint_ReturnsSuccess()
    {
        var loginRequest = new { email = "string", password = "string" };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = loginBody.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );

        var response = await _client.PostAsJsonAsync(
            "/api/product/addProduct",
            new
            {
                name = "Integration Test Product",
                description = "Test",
                price = 100,
                stockQuantity = 10,
                categoryId = "4AC14E37-02DD-47E3-BD29-46A76E4A5D32",
            }
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var loginRequest = new { email = "string", password = "string" };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();

        var token = loginBody.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token
        );
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/product/getProductById/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
