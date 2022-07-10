using GreenTubeDevTask.Controllers;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{

    public class PlayerService : IPlayerService
    {
        // public
        public PlayerService(ILogger<PlayerService> logger, IWalletService walletService)
        {
            _logger = logger;
            WalletService = walletService;
        }
        public IEnumerable<Player> GetPlayers()
        {
            return _repository.Get();
        }
        public Player GetPlayer(Guid id)
        {
            return _repository.GetById(id);
        }
        public Wallet GetPlayerWallet(Guid id)
        {
            return WalletService.GetWallet(id);
        }
#nullable enable
        public Player? CreatePlayer(PlayerRegisterContract player)
        {
            _logger.LogInformation("Creating Player with Username: {0}.", player.Username);
            if (_repository.UsernameExists(player.Username))
            {
                _logger.LogInformation("Failed to create Player, username exists. Username: {0}", player.Username);
                return null;
            }

            Guid newPlayerId = Guid.NewGuid();
            _ = WalletService.CreateWallet(newPlayerId);

            Player newPlayer = new()
            {
                Id = newPlayerId,
                Username = player.Username,
                DateCreated = DateTime.Now
            };
            _repository.Add(newPlayer);
            _logger.LogInformation("Created Player: {0}.", newPlayer);
            return newPlayer;
        }
#nullable disable

        // private, internal, protected
        private readonly ILogger<PlayerService> _logger;
        private readonly PlayerRepository _repository = new();
        internal readonly IWalletService WalletService;
        Player IPlayerService.GetPlayerByUsername(string username)
        {
            return _repository.GetByUsername(username);
        }
    }
}
