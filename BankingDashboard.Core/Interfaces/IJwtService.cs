using System.Security.Claims;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}
