using System;
using System.Collections.Generic;

namespace BankingDashboard.Core.Models
{
    /// <summary>
    /// Represents a user in the banking system
    /// </summary>
    public class User
    {
        public User()
        {
            Role = "User";
            Username = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
            Salt = string.Empty;
            Accounts = new List<Account>();
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
