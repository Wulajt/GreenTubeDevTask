using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{
    public interface IWalletService
    {
        IEnumerable<Wallet> GetWallets();
        Wallet GetWallet(Guid id);
        Wallet CreateWallet(Guid playerId);
#nullable enable
        Wallet? IncreaseWalletBalance(Guid id, decimal amount);
        Wallet? DecreaseWalletBalance(Guid id, decimal amount);
    }
}