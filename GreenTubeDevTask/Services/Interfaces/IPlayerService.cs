using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public interface IPlayerService
    {
        Task<IEnumerable<Player>> GetPlayersAync();
        Task<Player> GetPlayerAsync(Guid id);
#nullable enable
        Task<Player?> CreatePlayerAsync(PlayerRegisterContract player);
    }
}