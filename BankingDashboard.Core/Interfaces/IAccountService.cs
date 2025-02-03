using System.Collections.Generic;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetAccountByIdAsync(int id);
        Task<IEnumerable<Account>> GetUserAccountsAsync(int userId);
        Task<Account> CreateAccountAsync(Account account);
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int id);
        Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int userId, int count = 10);
    }
}
