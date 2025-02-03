using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;
using BankingDashboard.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace BankingDashboard.Tests.Integration
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsToken()
        {
            // Arrange
            var registerModel = new
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(registerModel),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { token = "" });
            Assert.NotNull(result.token);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginModel = new
            {
                Username = "testuser",
                Password = "Test123!"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginModel),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeAnonymousType(responseString, new { token = "" });
            Assert.NotNull(result.token);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.PostAsync("/api/auth/logout", null);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ReturnsSuccess()
        {
            // Arrange
            // First login to get a token
            var loginModel = new
            {
                Username = "testuser",
                Password = "Test123!"
            };

            var loginContent = new StringContent(
                JsonConvert.SerializeObject(loginModel),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
            var loginResponseString = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonConvert.DeserializeAnonymousType(loginResponseString, new { token = "" });

            // Add token to client
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.token);

            // Act
            var response = await _client.PostAsync("/api/auth/logout", null);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
