using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{
    public interface IPlayerService
    {
        IEnumerable<Player> GetPlayers();
        Player GetPlayer(Guid id);
        Player GetPlayerByUsername(string username);
        public Wallet GetPlayerWallet(Guid id);
#nullable enable
        Player? CreatePlayer(PlayerRegisterContract player);
    }
}