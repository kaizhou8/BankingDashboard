using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Core.Models;
using BankingDashboard.Core.Enums;
using BankingDashboard.Infrastructure.Services;

namespace BankingDashboard.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _accountServiceMock = new Mock<IAccountService>();
            _transactionService = new TransactionService(_transactionRepositoryMock.Object, _accountServiceMock.Object);
        }

        [Fact]
        public async Task CreateDepositAsync_WithValidAmount_ShouldCreateTransaction()
        {
            // Arrange
            var accountId = 1;
            var amount = 100m;
            var description = "Test deposit";
            var account = new Account { Id = accountId, Balance = 0 };
            var transaction = new Transaction 
            { 
                Id = 1,
                AccountId = accountId,
                Amount = amount,
                Type = TransactionType.Deposit,
                Description = description
            };

            _accountServiceMock.Setup(x => x.GetAccountByIdAsync(accountId))
                .ReturnsAsync(account);
            _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.CreateDepositAsync(accountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(TransactionType.Deposit, result.Type);
            Assert.Equal(description, result.Description);
            _accountServiceMock.Verify(x => x.UpdateAccountAsync(It.Is<Account>(a => a.Balance == amount)), Times.Once);
        }

        [Fact]
        public async Task CreateWithdrawalAsync_WithSufficientFunds_ShouldCreateTransaction()
        {
            // Arrange
            var accountId = 1;
            var initialBalance = 200m;
            var amount = 100m;
            var description = "Test withdrawal";
            var account = new Account { Id = accountId, Balance = initialBalance };
            var transaction = new Transaction 
            { 
                Id = 1,
                AccountId = accountId,
                Amount = -amount,
                Type = TransactionType.Withdrawal,
                Description = description
            };

            _accountServiceMock.Setup(x => x.GetAccountByIdAsync(accountId))
                .ReturnsAsync(account);
            _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.CreateWithdrawalAsync(accountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.AccountId);
            Assert.Equal(-amount, result.Amount);
            Assert.Equal(TransactionType.Withdrawal, result.Type);
            Assert.Equal(description, result.Description);
            _accountServiceMock.Verify(x => x.UpdateAccountAsync(It.Is<Account>(a => a.Balance == initialBalance - amount)), Times.Once);
        }

        [Fact]
        public async Task CreateWithdrawalAsync_WithInsufficientFunds_ShouldThrowException()
        {
            // Arrange
            var accountId = 1;
            var initialBalance = 50m;
            var amount = 100m;
            var description = "Test withdrawal";
            var account = new Account { Id = accountId, Balance = initialBalance };

            _accountServiceMock.Setup(x => x.GetAccountByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _transactionService.CreateWithdrawalAsync(accountId, amount, description));
        }

        [Fact]
        public async Task CreateTransferAsync_WithValidAmountAndAccounts_ShouldCreateTransactions()
        {
            // Arrange
            var fromAccountId = 1;
            var toAccountId = 2;
            var initialFromBalance = 200m;
            var initialToBalance = 0m;
            var amount = 100m;
            var description = "Test transfer";

            var fromAccount = new Account { Id = fromAccountId, Balance = initialFromBalance };
            var toAccount = new Account { Id = toAccountId, Balance = initialToBalance };

            var withdrawalTransaction = new Transaction
            {
                Id = 1,
                AccountId = fromAccountId,
                Amount = -amount,
                Type = TransactionType.Transfer,
                Description = $"Transfer to Account {toAccountId}: {description}"
            };

            var depositTransaction = new Transaction
            {
                Id = 2,
                AccountId = toAccountId,
                Amount = amount,
                Type = TransactionType.Transfer,
                Description = $"Transfer from Account {fromAccountId}: {description}"
            };

            _accountServiceMock.Setup(x => x.GetAccountByIdAsync(fromAccountId))
                .ReturnsAsync(fromAccount);
            _accountServiceMock.Setup(x => x.GetAccountByIdAsync(toAccountId))
                .ReturnsAsync(toAccount);
            _transactionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(withdrawalTransaction);

            // Act
            var result = await _transactionService.CreateTransferAsync(fromAccountId, toAccountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fromAccountId, result.AccountId);
            Assert.Equal(-amount, result.Amount);
            Assert.Equal(TransactionType.Transfer, result.Type);
            _accountServiceMock.Verify(x => x.UpdateAccountAsync(It.Is<Account>(a => a.Balance == initialFromBalance - amount)), Times.Once);
            _accountServiceMock.Verify(x => x.UpdateAccountAsync(It.Is<Account>(a => a.Balance == initialToBalance + amount)), Times.Once);
        }

        [Fact]
        public async Task GetUserTransactionsAsync_ShouldReturnAllUserTransactions()
        {
            // Arrange
            var userId = 1;
            var accounts = new List<Account>
            {
                new Account { Id = 1, UserId = userId },
                new Account { Id = 2, UserId = userId }
            };

            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, AccountId = 1, Amount = 100 },
                new Transaction { Id = 2, AccountId = 1, Amount = -50 },
                new Transaction { Id = 3, AccountId = 2, Amount = 200 }
            };

            _accountServiceMock.Setup(x => x.GetUserAccountsAsync(userId))
                .ReturnsAsync(accounts);
            _transactionRepositoryMock.Setup(x => x.GetByAccountIdAsync(1))
                .ReturnsAsync(transactions.Where(t => t.AccountId == 1));
            _transactionRepositoryMock.Setup(x => x.GetByAccountIdAsync(2))
                .ReturnsAsync(transactions.Where(t => t.AccountId == 2));

            // Act
            var result = await _transactionService.GetUserTransactionsAsync(userId);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(result, t => t.Id == 1);
            Assert.Contains(result, t => t.Id == 2);
            Assert.Contains(result, t => t.Id == 3);
        }
    }
}
