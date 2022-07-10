﻿using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask
{
    public static class Extensions
    {
        public static PlayerContract AsContract(this Player player)
        {
            return new PlayerContract(player.Id, player.Username, player.DateCreated);
        }

        public static WalletContract AsContract(this Wallet wallet)
        {
            return new WalletContract(wallet.Id, wallet.Balance, wallet.DateCreated);
        }
    }
}
