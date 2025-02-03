using System.Collections.Generic;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateDepositAsync(int accountId, decimal amount, string description);
        Task<Transaction> CreateWithdrawalAsync(int accountId, decimal amount, string description);
        Task<Transaction> CreateTransferAsync(int fromAccountId, int toAccountId, decimal amount, string description);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
    }
}
