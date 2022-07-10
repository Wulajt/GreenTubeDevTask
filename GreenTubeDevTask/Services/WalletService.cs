using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using System.Collections.Generic;
using System;
using GreenTubeDevTask.Controllers;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace GreenTubeDevTask.Services
{
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly WalletRepository _repository = new();
        public WalletService(ILogger<WalletService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Wallet> GetWallets()
        {
            return _repository.Get();
        }
        public Wallet GetWallet(Guid id)
        {
            return _repository.GetById(id);
        }
        Wallet IWalletService.CreateWallet(Guid playerId)
        {
            _logger.LogInformation("Creating Wallet for PlayerId: {0}.", playerId);
            Wallet newWallet = new()
            {
                Id = playerId,
                Balance = 0,
                DateCreated = DateTime.Now
            };
            _repository.Add(newWallet);
            _logger.LogInformation("Created Wallet: {0}.", newWallet);
            return newWallet;
        }
    }
}