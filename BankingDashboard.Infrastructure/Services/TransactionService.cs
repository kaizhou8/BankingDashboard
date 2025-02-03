using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Core.Models;
using BankingDashboard.Core.Enums;

namespace BankingDashboard.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountService _accountService;

        public TransactionService(ITransactionRepository transactionRepository, IAccountService accountService)
        {
            _transactionRepository = transactionRepository;
            _accountService = accountService;
        }

        public async Task<Transaction> CreateDepositAsync(int accountId, decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new ArgumentException("Account not found", nameof(accountId));

            account.Balance += amount;
            await _accountService.UpdateAccountAsync(account);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Type = TransactionType.Deposit,
                Description = description,
                TransactionDate = DateTime.UtcNow
            };

            return await _transactionRepository.CreateAsync(transaction);
        }

        public async Task<Transaction> CreateWithdrawalAsync(int accountId, decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new ArgumentException("Account not found", nameof(accountId));

            if (account.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            account.Balance -= amount;
            await _accountService.UpdateAccountAsync(account);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = -amount,
                Type = TransactionType.Withdrawal,
                Description = description,
                TransactionDate = DateTime.UtcNow
            };

            return await _transactionRepository.CreateAsync(transaction);
        }

        public async Task<Transaction> CreateTransferAsync(int fromAccountId, int toAccountId, decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            if (fromAccountId == toAccountId)
                throw new ArgumentException("Cannot transfer to the same account");

            var fromAccount = await _accountService.GetAccountByIdAsync(fromAccountId);
            var toAccount = await _accountService.GetAccountByIdAsync(toAccountId);

            if (fromAccount == null)
                throw new ArgumentException("Source account not found", nameof(fromAccountId));

            if (toAccount == null)
                throw new ArgumentException("Destination account not found", nameof(toAccountId));

            if (fromAccount.Balance < amount)
                throw new InvalidOperationException("Insufficient funds");

            // Create withdrawal transaction
            var withdrawalTransaction = new Transaction
            {
                AccountId = fromAccountId,
                Amount = -amount,
                Type = TransactionType.Transfer,
                Description = $"Transfer to account {toAccountId}: {description}",
                TransactionDate = DateTime.UtcNow
            };

            // Create deposit transaction
            var depositTransaction = new Transaction
            {
                AccountId = toAccountId,
                Amount = amount,
                Type = TransactionType.Transfer,
                Description = $"Transfer from account {fromAccountId}: {description}",
                TransactionDate = DateTime.UtcNow
            };

            // Update account balances
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            // Save all changes
            await _accountService.UpdateAccountAsync(fromAccount);
            await _accountService.UpdateAccountAsync(toAccount);
            await _transactionRepository.CreateAsync(withdrawalTransaction);
            return await _transactionRepository.CreateAsync(depositTransaction);
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            var accounts = await _accountService.GetUserAccountsAsync(userId);
            var transactions = new List<Transaction>();

            foreach (var account in accounts)
            {
                var accountTransactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
                transactions.AddRange(accountTransactions);
            }

            return transactions;
        }

        public async Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(int accountId)
        {
            return await _transactionRepository.GetByAccountIdAsync(accountId);
        }
    }
}
