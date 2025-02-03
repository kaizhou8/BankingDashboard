using Microsoft.AspNetCore.Mvc;
using BankingDashboard.Core.Models;
using BankingDashboard.Core.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace BankingDashboard.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountService _accountService;

        public TransactionController(
            ITransactionService transactionService,
            ITransactionRepository transactionRepository,
            IAccountService accountService)
        {
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return NotFound();

            // Verify if the user has permission to access this transaction
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            if (transaction.Account.UserId != userId)
                return Forbid();

            return Ok(transaction);
        }

        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAccountTransactionsAsync(int accountId)
        {
            // Verify if the user has permission to access transactions for this account
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var account = await _accountService.GetAccountByIdAsync(accountId);
            
            if (account == null)
                return NotFound();
                
            if (account.UserId != userId)
                return Forbid();
            
            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
            return Ok(transactions);
        }

        [HttpPost("deposit")]
        public async Task<ActionResult<Transaction>> DepositAsync([FromBody] TransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var account = await _accountService.GetAccountByIdAsync(request.AccountId);
                
                if (account == null)
                    return NotFound();
                    
                if (account.UserId != userId)
                    return Forbid();

                var transaction = await _transactionService.CreateDepositAsync(
                    request.AccountId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransactionAsync), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult<Transaction>> WithdrawAsync([FromBody] TransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var account = await _accountService.GetAccountByIdAsync(request.AccountId);
                
                if (account == null)
                    return NotFound();
                    
                if (account.UserId != userId)
                    return Forbid();

                var transaction = await _transactionService.CreateWithdrawalAsync(
                    request.AccountId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransactionAsync), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<Transaction>> TransferAsync([FromBody] TransferRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var fromAccount = await _accountService.GetAccountByIdAsync(request.FromAccountId);
                var toAccount = await _accountService.GetAccountByIdAsync(request.ToAccountId);
                
                if (fromAccount == null || toAccount == null)
                    return NotFound();
                    
                if (fromAccount.UserId != userId)
                    return Forbid();

                var transaction = await _transactionService.CreateTransferAsync(
                    request.FromAccountId,
                    request.ToAccountId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetTransactionAsync), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class TransactionRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class TransferRequest
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}