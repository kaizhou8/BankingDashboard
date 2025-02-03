using System;
using BankingDashboard.Core.Enums;

namespace BankingDashboard.Core.Models
{
    /// <summary>
    /// Represents a financial transaction in the system
    /// </summary>
    public class Transaction
    {
        public Transaction()
        {
            Description = string.Empty;
            TransactionDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType Type { get; set; }
        
        // Navigation property
        public virtual Account Account { get; set; }
    }
}
