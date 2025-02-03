using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Core.Models;
using BankingDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BankingDashboard.Web;

namespace BankingDashboard.Tests.Integration;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnToken()
    {
        // Arrange
        var registerData = new
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/login/register", registerData);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result?.Token);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var registerData = new
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "Login123!"
        };
        await _client.PostAsJsonAsync("/api/login/register", registerData);

        var loginData = new
        {
            Username = "loginuser",
            Password = "Login123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/login/login", loginData);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result?.Token);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Dashboard_WithValidToken_ShouldReturnData()
    {
        // Arrange
        var registerData = new
        {
            Username = "dashboarduser",
            Email = "dashboard@example.com",
            Password = "Dashboard123!"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/login/register", registerData);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var tokenResult = JsonSerializer.Deserialize<TokenResponse>(registerContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult?.Token);

        // Act
        var response = await _client.GetAsync("/api/dashboard");
        var content = await response.Content.ReadAsStringAsync();
        var dashboardData = JsonSerializer.Deserialize<DashboardData>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dashboardData);
        Assert.Equal("dashboarduser", dashboardData.UserName);
    }

    [Fact]
    public async Task Dashboard_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private class TokenResponse
    {
        public string Token { get; set; }
    }

    private class DashboardData
    {
        public string UserName { get; set; }
        public string LastLoginTime { get; set; }
        public decimal TotalBalance { get; set; }
        public List<AccountData> Accounts { get; set; }
        public List<TransactionData> RecentTransactions { get; set; }
    }

    private class AccountData
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
    }

    private class TransactionData
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
