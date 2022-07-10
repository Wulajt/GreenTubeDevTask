using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<PlayerContract> GetPlayers()
        {
            var players = _playerService.GetPlayers().Select(item => item.AsContract());
            return players;
        }

        [HttpGet("{id}")]
        public ActionResult<PlayerContract> GetPlayer(Guid id)
        {
            var player = _playerService.GetPlayer(id);
            if (player is null) return NotFound();
            return player.AsContract();
        }

        [HttpPost("register")]
        public ActionResult<PlayerContract> RegisterPlayer(PlayerRegisterContract playerContract)
        {
            var newPlayer = _playerService.CreatePlayer(playerContract);
            if (newPlayer is null) return ValidationProblem(detail: "Username already registered.");
            return CreatedAtAction(nameof(GetPlayer), new { id = newPlayer.Id }, newPlayer.AsContract());
        }
    }
}
