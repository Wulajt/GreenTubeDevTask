using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{
    public interface IWalletService
    {
        IEnumerable<Wallet> GetWallets();
        Wallet GetWallet(Guid id);
        internal Wallet CreateWallet(Guid playerId);
    }
}