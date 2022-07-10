using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<WalletContract> GetWallets()
        {
            var wallets = _walletService.GetWallets().Select(wallet => wallet.AsContract());
            return wallets;
        }

        [HttpGet("{id}")]
        public ActionResult<WalletContract> GetWallet(Guid id)
        {
            var wallet = _walletService.GetWallet(id);
            return wallet is null ? NotFound() : wallet.AsContract();
        }

        [HttpPost("{id}/transaction")]
        public ActionResult<TransactionContract> RegisterTransaction(RegisterTransactionContract registerTransaction)
        {
            var result = _transactionService.RegisterTransaction(registerTransaction);
            return result is null ? NotFound() : result.AsContract();
        }
    }
}
