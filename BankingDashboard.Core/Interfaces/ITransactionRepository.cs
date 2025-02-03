using System.Collections.Generic;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
        Task<IEnumerable<Transaction>> GetByAccountIdsAsync(IEnumerable<int> accountIds);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
    }
}
