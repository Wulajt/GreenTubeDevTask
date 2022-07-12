using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Player>> GetPlayersAync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<Player> GetPlayerAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<Wallet> GetPlayerWallet(Guid id)
        {
            return await _walletService.GetWalletAsync(id);
        }
#nullable enable
        public async Task<Player?> CreatePlayerAsync(PlayerRegisterContract player)
        {
            _logger.LogInformation("Creating Player with Username: {0}.", player.Username);
            if (await _repository.GetByUsernameAsync(player.Username) is not null)
            {
                _logger.LogInformation("Failed to create Player, username exists. Username: {0}", player.Username);
                return null;
            }

            Guid newPlayerId = Guid.NewGuid();
            _ = _walletService.CreateWalletAsync(newPlayerId);

            Player newPlayer = new()
            {
                Id = newPlayerId,
                Username = player.Username,
                DateCreated = DateTime.Now
            };
            await _repository.AddAsync(newPlayer);
            _logger.LogInformation("Created Player: {0}.", newPlayer);
            return await Task.FromResult(newPlayer);
        }
#nullable disable

        // private, internal, protected
        private readonly ILogger<PlayerService> _logger;
        private readonly IPlayerRepository _repository;
        private readonly IWalletService _walletService;
    }
}
