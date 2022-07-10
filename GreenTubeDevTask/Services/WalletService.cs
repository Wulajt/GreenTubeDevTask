using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{
    public class WalletService : IWalletService
    {
        //Public
        public WalletService(ILogger<WalletService> logger, IWalletRepository walletRepository)
        {
            _logger = logger;
            _repository = walletRepository;
        }
        public IEnumerable<Wallet> GetWallets()
        {
            return _repository.GetAll();
        }
        public Wallet GetWallet(Guid id)
        {
            return _repository.GetById(id);
        }
        public Wallet CreateWallet(Guid playerId)
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
#nullable enable
        public bool? IncreaseWalletBalance(Guid id, decimal amount)
        {
            var wallet = GetWallet(id);
            if (wallet is null) return null;
            wallet.Balance = decimal.Add(wallet.Balance, amount);
            return true;
        }

        public bool? DecreaseWalletBalance(Guid id, decimal amount)
        {
            var wallet = GetWallet(id);
            if (wallet is null) return null;
            if (wallet.Balance < amount) return false;
            wallet.Balance = decimal.Subtract(wallet.Balance, amount);
            return true;
        }
#nullable disable

        // Private, Internal, Protected
        private readonly ILogger<WalletService> _logger;
        private readonly IWalletRepository _repository;
    }
}