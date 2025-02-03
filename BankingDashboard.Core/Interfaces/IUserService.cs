using System.Threading.Tasks;
using BankingDashboard.Core.Models;

namespace BankingDashboard.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User> RegisterUserAsync(string username, string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
