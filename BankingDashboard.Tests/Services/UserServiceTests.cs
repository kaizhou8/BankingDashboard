using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using BankingDashboard.Infrastructure.Services;
using BankingDashboard.Core.Models;
using BankingDashboard.Infrastructure.Data;
using BankingDashboard.Core.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BankingDashboard.Tests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;
        private readonly User _testUser;

        public UserServiceTests()
        {
            _testUser = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedPassword123",
                Salt = "testSalt"
            };

            _userRepositoryMock = new Mock<IUserRepository>();
            _dbContextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(_testUser);
            _userRepositoryMock.Setup(x => x.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(_testUser);
            _userRepositoryMock.Setup(x => x.GetByUsernameAsync("testuser"))
                .ReturnsAsync(_testUser);

            _userService = new UserService(_userRepositoryMock.Object, _dbContextMock.Object, _loggerMock.Object);
        }

        public void Dispose()
        {
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testUser.Username, result.Username);
            Assert.Equal(_testUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser()
        {
            // Act
            var result = await _userService.GetUserByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testUser.Id, result.Id);
            Assert.Equal(_testUser.Username, result.Username);
        }

        [Fact]
        public async Task RegisterUserAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var newUser = new User
            {
                Id = 2,
                Username = "newuser",
                Email = "new@example.com",
                PasswordHash = "hashedPassword456",
                Salt = "newSalt"
            };

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(newUser);

            // Act
            var result = await _userService.RegisterUserAsync("newuser", "new@example.com", "password456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Id, result.Id);
            Assert.Equal(newUser.Username, result.Username);
            Assert.Equal(newUser.Email, result.Email);
        }

        [Fact]
        public async Task ValidateUserAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "hashedPassword123",
                Salt = "testSalt"
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .Returns<string>(username => Task.FromResult(user));
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns<User>(u => Task.FromResult(u));

            // Act
            var result = await _userService.ValidateUserAsync("testuser", "password123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidCurrentPassword_ShouldReturnTrue()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "hashedPassword123",
                Salt = "testSalt"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(user));
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns<User>(u => Task.FromResult(u));

            // Act
            var result = await _userService.ChangePasswordAsync(1, "password123", "newpassword123");

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
