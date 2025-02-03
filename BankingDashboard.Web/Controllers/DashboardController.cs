using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankingDashboard.Core.Interfaces;
using BankingDashboard.Web.Models;
using System.Security.Claims;

namespace BankingDashboard.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public DashboardController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var accounts = await _accountService.GetUserAccountsAsync(userId);
            var transactions = await _accountService.GetRecentTransactionsAsync(userId);
            var viewModel = new DashboardViewModel(user, accounts, transactions);

            return View(viewModel);
        }
    }
}
