using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Controllers
{
    [ApiController]
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;
        public PlayersController(ILogger<PlayersController> logger, IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IEnumerable<PlayerContract>> GetPlayersAsync()
        {
            var players = (await _playerService.GetPlayersAync()).Select(item => item.AsContract());
            return players;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerContract>> GetPlayerAsync(Guid id)
        {
            var player = await _playerService.GetPlayerAsync(id);
            return player is null ? NotFound() : player.AsContract();
        }

        [HttpPost("register")]
        public async Task<ActionResult<PlayerContract>> RegisterPlayerAsync(PlayerRegisterContract playerContract)
        {
            var newPlayer = await _playerService.CreatePlayerAsync(playerContract);
            if (newPlayer is null) return ValidationProblem(detail: "Username already registered.");
            return CreatedAtAction(nameof(GetPlayerAsync), new { id = newPlayer.Id }, newPlayer.AsContract());
        }
    }
}
