using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public class TransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private readonly TransactionRepository _repository = new();
        private readonly IWalletService _walletService;
        public TransactionService(ILogger<TransactionService> logger, IWalletService walletService)
        {
            _logger = logger;
            _walletService = walletService;
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            return _repository.Get();
        }
        public IEnumerable<Transaction> GetTransactionsByPlayerId(Guid playerId)
        {
            return _repository.GetAllByPlayerId(playerId);
        }
        public Transaction GetTransaction(Guid id)
        {
            return _repository.GetById(id);
        }
        //public Transaction CreateTransaction(TransactionType type, decimal amount, Guid walletId)
        //{
        //    _logger.LogInformation("Creating Transaction, WalletId: {0}, Type: {1}, Amount: {2}.", walletId, type, amount);
        //    var wallet = _walletService.GetWallet(walletId);
        //    if (_repository.UsernameExists(player.Username))
        //    {
        //        _logger.LogInformation("Failed to create Player, username exists. Username: {0}", player.Username);
        //        return null;
        //    }

        //    Guid newPlayerId = Guid.NewGuid();
        //    var newWallet = _walletService.CreateWallet(newPlayerId);

        //    Player newPlayer = new()
        //    {
        //        Id = newPlayerId,
        //        WalletId = newWallet.Id,
        //        Username = player.Username,
        //        DateCreated = DateTime.Now
        //    };
        //    _repository.Add(newPlayer);
        //    _logger.LogInformation("Created Player: {0}.", newPlayer);
        //    return newPlayer;
        //}
    }
}
