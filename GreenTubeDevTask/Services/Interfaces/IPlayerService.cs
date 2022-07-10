using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{
    public interface IPlayerService
    {
        IEnumerable<Player> GetPlayers();
        Player GetPlayer(Guid id);
        Player GetPlayerByUsername(string username);
#nullable enable
        Player? CreatePlayer(PlayerRegisterContract player);
    }
}