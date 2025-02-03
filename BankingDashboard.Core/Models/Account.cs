using System;
using System.Collections.Generic;

namespace BankingDashboard.Core.Models
{
    /// <summary>
    /// Represents a bank account in the system
    /// </summary>
    public class Account
    {
        public Account()
        {
            AccountNumber = Guid.NewGuid().ToString("N").Substring(0, 10);
            AccountType = "Savings";
            Transactions = new List<Transaction>();
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public User? User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
