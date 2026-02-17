using AppointMed.API.Models.Account;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository repository;
        private readonly ILogger<AccountsController> logger;

        public AccountsController(IAccountRepository repository, ILogger<AccountsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        // GET: api/Accounts/MyAccount
        [HttpGet("MyAccount")]
        public async Task<ActionResult<AccountDto>> GetMyAccount()
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var account = await repository.GetAccountByUserIdAsync(userId);

                if (account == null)
                {
                    // Create account if it doesn't exist
                    await repository.GetOrCreateAccountAsync(userId);
                    account = await repository.GetAccountByUserIdAsync(userId);
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetMyAccount)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Accounts/MyTransactions
        [HttpGet("MyTransactions")]
        public async Task<ActionResult<List<AccountTransactionDto>>> GetMyTransactions()
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var account = await repository.GetAccountByUserIdAsync(userId);
                if (account == null)
                    return Ok(new List<AccountTransactionDto>());

                var transactions = await repository.GetTransactionsByAccountIdAsync(account.AccountId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetMyTransactions)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Accounts/User/{userId} - Admin only
        [HttpGet("User/{userId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<AccountDto>> GetAccountByUserId(string userId)
        {
            try
            {
                var account = await repository.GetAccountByUserIdAsync(userId);
                if (account == null)
                    return NotFound();

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetAccountByUserId)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
    }
}