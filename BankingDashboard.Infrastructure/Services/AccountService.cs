using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingDashboard.Core.Models;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingDashboard.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetUserAccountsAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Include(a => a.Transactions)
                .ToListAsync();
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return false;

            _context.Accounts.Remove(account);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UpdateBalanceAsync(int accountId, decimal amount)
        {
            var account = await GetAccountByIdAsync(accountId);
            account.Balance = amount;
            await UpdateAccountAsync(account);
        }

        public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(int userId, int count = 10)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
