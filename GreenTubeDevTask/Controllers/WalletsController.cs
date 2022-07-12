using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Controllers
{
    [ApiController]
    [Route("wallets")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<WalletsController> _logger;
        public WalletsController(ILogger<WalletsController> logger, IWalletService walletService, ITransactionService transactionService)
        {
            _logger = logger;
            _walletService = walletService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IEnumerable<WalletContract>> GetWalletsAsync()
        {
            var wallets = (await _walletService.GetWalletsAsync()).Select(wallet => wallet.AsContract());
            return wallets;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WalletContract>> GetWalletAsync(Guid id)
        {
            var wallet = await _walletService.GetWalletAsync(id);
            return wallet is null ? NotFound() : wallet.AsContract();
        }

        [HttpPost("transactions/register")]
        public async Task<ActionResult<TransactionContract>> RegisterTransactionAsync(RegisterTransactionContract registerTransaction)
        {
            var result = await _transactionService.RegisterTransactionAsync(registerTransaction);
            return result is null ? NotFound() : result.AsContract();
        }

        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionContract>>> GetWalletTransactionsAsync(Guid id)
        {
            var result = await _transactionService.GetTransactionsByPlayerIdAsync(id);
            return result is null ? NotFound() : Ok(result.Select(x => x.AsContract()));
        }
    }
}
