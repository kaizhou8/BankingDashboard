using System;
using System.Threading.Tasks;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Core.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankingDashboard.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace BankingDashboard.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ApplicationDbContext context, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id) ?? throw new ArgumentException("User not found", nameof(id));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email) ?? throw new ArgumentException("User not found", nameof(email));
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username) ?? throw new ArgumentException("User not found", nameof(username));
        }

        public async Task<User> UpdateLastLoginAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found", nameof(userId));

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            // Validate user data
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required", nameof(user.Email));

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required", nameof(user.Username));

            // Set default values
            user.CreatedAt = DateTime.UtcNow;
            user.LastLoginAt = DateTime.UtcNow;
            user.IsActive = true;

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            var hashedPassword = HashPassword(password);
            if (user.PasswordHash != hashedPassword)
                return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

            existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var currentHashedPassword = HashPassword(currentPassword);
            if (user.PasswordHash != currentHashedPassword)
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
