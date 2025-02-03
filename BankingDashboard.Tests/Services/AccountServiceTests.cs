using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;
using BankingDashboard.Infrastructure.Services;
using BankingDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingDashboard.Tests.Services
{
    public class AccountServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _accountService = new AccountService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAccountByIdAsync_ShouldReturnAccount()
        {
            // Arrange
            var account = new Account { Id = 1, Balance = 1000m };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            var result = await _accountService.GetAccountByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1000m, result.Balance);
        }

        [Fact]
        public async Task GetUserAccountsAsync_ShouldReturnUserAccounts()
        {
            // Arrange
            var userId = 1;
            var accounts = new List<Account>
            {
                new Account { Id = 1, UserId = userId, Balance = 1000m },
                new Account { Id = 2, UserId = userId, Balance = 2000m },
                new Account { Id = 3, UserId = 2, Balance = 3000m }
            };
            _context.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _accountService.GetUserAccountsAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, a => Assert.Equal(userId, a.UserId));
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldCreateNewAccount()
        {
            // Arrange
            var account = new Account
            {
                UserId = 1,
                Balance = 0,
                AccountNumber = "123456",
                AccountType = "Savings",
                IsActive = true
            };

            // Act
            var result = await _accountService.CreateAccountAsync(account);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            var savedAccount = await _context.Accounts.FindAsync(result.Id);
            Assert.NotNull(savedAccount);
            Assert.Equal(account.Balance, savedAccount.Balance);
        }

        [Fact]
        public async Task UpdateBalanceAsync_ShouldUpdateAccountBalance()
        {
            // Arrange
            var account = new Account { Id = 1, Balance = 1000m };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Act
            await _accountService.UpdateBalanceAsync(1, 500m);

            // Assert
            var updatedAccount = await _context.Accounts.FindAsync(1);
            Assert.NotNull(updatedAccount);
            Assert.Equal(1500m, updatedAccount.Balance);
        }

        [Fact]
        public async Task UpdateBalanceAsync_WithInvalidAccount_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _accountService.UpdateBalanceAsync(999, 500m));
        }
    }
}
