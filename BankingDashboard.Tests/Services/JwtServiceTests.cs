using Xunit;
using Microsoft.Extensions.Configuration;
using BankingDashboard.Infrastructure.Services;
using BankingDashboard.Core.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace BankingDashboard.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public JwtServiceTests()
        {
            var configValues = new Dictionary<string, string>
            {
                {"JwtSettings:SecretKey", "your-256-bit-secret-key-for-testing"},
                {"JwtSettings:Issuer", "test-issuer"},
                {"JwtSettings:Audience", "test-audience"},
                {"JwtSettings:ExpirationMinutes", "60"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _jwtService = new JwtService(_configuration);
        }

        [Fact]
        public void GenerateToken_ShouldCreateValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            // Act
            var token = _jwtService.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal("test-issuer", jwtToken.Issuer);
            Assert.Equal("test-audience", jwtToken.Audiences.First());
            Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == "unique_name" && c.Value == user.Username);
            Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == user.Email);
        }

        [Fact]
        public void GenerateToken_WithNullUser_ShouldReturnNull()
        {
            // Act
            var token = _jwtService.GenerateToken(null);

            // Assert
            Assert.Null(token);
        }
    }
}
