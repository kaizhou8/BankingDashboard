using System;
using System.Collections.Generic;
using System.Linq;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Web.Models
{
    public class DashboardViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string LastLoginTime { get; set; }
        public IEnumerable<Account> Accounts { get; set; } = new List<Account>();
        public IEnumerable<Transaction> RecentTransactions { get; set; } = new List<Transaction>();
        public decimal TotalBalance => CalculateTotalBalance();

        public DashboardViewModel(User user, IEnumerable<Account> accounts, IEnumerable<Transaction> transactions)
        {
            UserId = user.Id;
            Username = user.Username;
            Email = user.Email;
            LastLoginTime = user.LastLoginAt?.ToString("g") ?? "Never";
            Accounts = accounts ?? new List<Account>();
            RecentTransactions = transactions ?? new List<Transaction>();
        }

        private decimal CalculateTotalBalance()
        {
            decimal total = 0;
            foreach (var account in Accounts)
            {
                total += account.Balance;
            }
            return total;
        }
    }
}
