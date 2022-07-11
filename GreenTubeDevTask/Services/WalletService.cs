using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Wallet>> GetWalletsAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Wallet> GetWalletAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Wallet> CreateWalletAsync(Guid playerId)
        {
            _logger.LogInformation("Creating Wallet for PlayerId: {0}.", playerId);
            Wallet newWallet = new()
            {
                Id = playerId,
                Balance = 0,
                DateCreated = DateTime.Now
            };
            await _repository.AddAsync(newWallet);
            _logger.LogInformation("Created Wallet: {0}.", newWallet);
            return await Task.FromResult(newWallet);
        }
#nullable enable
        public async Task<bool?> IncreaseWalletBalanceAsync(Guid id, decimal amount)
        {
            var wallet = await GetWalletAsync(id);
            if (wallet is null) return null;
            mutexLock.WaitOne();
            wallet.Balance = decimal.Add(wallet.Balance, amount);
            mutexLock.ReleaseMutex();
            return true;
        }

        public async Task<bool?> DecreaseWalletBalanceAsync(Guid id, decimal amount)
        {
            var wallet = await GetWalletAsync(id);
            if (wallet is null) return null;
            //bool result = false;
            mutexLock.WaitOne();
            bool result = false;
            decimal updatedBalance = decimal.Subtract(wallet.Balance, amount);
            if (updatedBalance >= decimal.Zero)
            {
                wallet.Balance = updatedBalance;
                result = true;
            }
            mutexLock.ReleaseMutex();
            return result;
        }
#nullable disable

        // Private, Internal, Protected
        private readonly ILogger<WalletService> _logger;
        private readonly IWalletRepository _repository;
        private static Mutex mutexLock = new Mutex(false);
    }
}