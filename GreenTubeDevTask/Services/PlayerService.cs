using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.Services
{

    public class PlayerService : IPlayerService
    {
        // public
        public PlayerService(ILogger<PlayerService> logger, IWalletService walletService, IPlayerRepository playerRepository)
        {
            _logger = logger;
            _walletService = walletService;
            _repository = playerRepository;
        }
        public IEnumerable<Player> GetPlayers()
        {
            return _repository.GetAll();
        }
        public Player GetPlayer(Guid id)
        {
            return _repository.GetById(id);
        }
        public Wallet GetPlayerWallet(Guid id)
        {
            return _walletService.GetWallet(id);
        }
#nullable enable
        public Player? CreatePlayer(PlayerRegisterContract player)
        {
            _logger.LogInformation("Creating Player with Username: {0}.", player.Username);
            if (_repository.GetByUsername(player.Username) is not null)
            {
                _logger.LogInformation("Failed to create Player, username exists. Username: {0}", player.Username);
                return null;
            }

            Guid newPlayerId = Guid.NewGuid();
            _ = _walletService.CreateWallet(newPlayerId);

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
        private readonly IPlayerRepository _repository;
        private readonly IWalletService _walletService;
        Player IPlayerService.GetPlayerByUsername(string username)
        {
            return _repository.GetByUsername(username);
        }
    }
}
