using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetWalletsAsync();
        Task<Wallet> GetWalletAsync(Guid id);
        Task<Wallet> CreateWalletAsync(Guid playerId);
#nullable enable
        Task<bool?> IncreaseWalletBalanceAsync(Guid id, decimal amount);
        Task<bool?> DecreaseWalletBalanceAsync(Guid id, decimal amount);
    }
}