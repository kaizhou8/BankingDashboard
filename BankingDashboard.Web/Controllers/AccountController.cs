using Microsoft.AspNetCore.Mvc;
using BankingDashboard.Core.Models;
using BankingDashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BankingDashboard.Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BankingDashboard.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AccountController(IAccountService accountService, IUserService userService, IJwtService jwtService)
        {
            _accountService = accountService;
            _userService = userService;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpGet("api/account/{id}")]
        public async Task<ActionResult<Account>> GetAccountAsync(string id)
        {
            if (!int.TryParse(id, out int accountId))
            {
                return BadRequest("Invalid account ID");
            }

            var account = await _accountService.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            // Ensure user can only access their own accounts
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (account.UserId != userId)
                return Forbid();

            return account;
        }

        [Authorize]
        [HttpGet("api/account/user/{userId}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetUserAccountsAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
            {
                return BadRequest("Invalid user ID");
            }

            // Ensure user can only access their own accounts
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (id != currentUserId)
                return Forbid();

            var accounts = await _accountService.GetUserAccountsAsync(id);
            return Ok(accounts);
        }

        [Authorize]
        [HttpPost("api/account")]
        public async Task<ActionResult<Account>> CreateAccountAsync([FromBody] CreateAccountViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Forbid();

            try
            {
                var account = new Account
                {
                    UserId = userId,
                    AccountType = request.AccountType,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    AccountNumber = GenerateAccountNumber()
                };
                
                var createdAccount = await _accountService.CreateAccountAsync(account);
                return CreatedAtAction(nameof(GetAccountAsync), new { id = createdAccount.Id }, createdAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateAccountNumber()
        {
            return DateTime.UtcNow.Ticks.ToString().Substring(0, 10);
        }

        [Authorize]
        [HttpGet("api/account/{id}/balance")]
        public async Task<ActionResult<decimal>> GetAccountBalanceAsync(string id)
        {
            if (!int.TryParse(id, out int accountId))
            {
                return BadRequest("Invalid account ID");
            }

            var account = await _accountService.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            // Ensure user can only access their own accounts
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (account.UserId != userId)
                return Forbid();

            return account.Balance;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userService.RegisterUserAsync(model.Username, model.Email, model.Password);
                var token = _jwtService.GenerateToken(user);

                // 创建身份验证票据
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userService.ValidateUserAsync(model.Username, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                var token = _jwtService.GenerateToken(user);

                // 创建身份验证票据
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

    public class CreateAccountViewModel
    {
        [Required]
        public string AccountType { get; set; } = string.Empty;
    }
}