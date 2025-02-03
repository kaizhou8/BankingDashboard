using System.Collections.Generic;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id);
        Task<IEnumerable<Account>> GetByUserIdAsync(int userId);
        Task<Account> CreateAsync(Account account);
        Task UpdateAsync(Account account);
    }
}
