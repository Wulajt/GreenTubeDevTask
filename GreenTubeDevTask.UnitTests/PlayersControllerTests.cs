using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Controllers;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class PlayersControllerTests
    {
        private readonly Mock<IPlayerService> _playerServiceStub = new();
        private readonly Mock<ILogger<PlayersController>> _loggerStub = new();

        [Fact]
        public async Task GetPlayersAsync_WithExistingPlayers_ReturnsAllPlayers()
        {
            // Arrange
            var expectedPlayers = new[] { CreateRandomPlayer(), CreateRandomPlayer(), CreateRandomPlayer() };
            _playerServiceStub.Setup(exp => exp.GetPlayersAync())
                .ReturnsAsync(expectedPlayers);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = await controller.GetPlayersAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedPlayers);
        }

        [Fact]
        public async Task GetPlayerAsync_WithNonExistingPlayer_ReturnsNotFound()
        {
            // Arrange
            _playerServiceStub.Setup(exp => exp.GetPlayerAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Player)null);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = await controller.GetPlayerAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetPlayerAsync_WithExistingPlayer_ReturnsExpectedPlayer()
        {
            // Arrange
            var expectedPlayer = CreateRandomPlayer();
            _playerServiceStub.Setup(exp => exp.GetPlayerAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedPlayer);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = await controller.GetPlayerAsync(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedPlayer);
        }

        [Fact]
        public async Task RegisterPlayerAsync_WithPlayerToRegisterWithExistingUsernamePlayerToRegister_ReturnsValidationProblem()
        {
            // Arrange
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());
            _playerServiceStub.Setup(exp => exp.CreatePlayerAsync(It.IsAny<PlayerRegisterContract>()))
                .ReturnsAsync((Player)null);

            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var registerResult = await controller.RegisterPlayerAsync(playerToRegister);

            // Assert
            var badRequestResult = registerResult.Result.Should().BeOfType<ObjectResult>().Which;
            var problemDetails = badRequestResult.Value.Should().BeOfType<ValidationProblemDetails>().Which;
            problemDetails.Detail.Should().Be("Username already registered.");
        }

        [Fact]
        public async Task RegisterPlayerAsync_WithPlayerToRegister_ReturnsRegisteredPlayer()
        {
            // Arrange
            var expectedPlayer = CreateRandomPlayer();
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());
            _playerServiceStub.Setup(exp => exp.CreatePlayerAsync(It.IsAny<PlayerRegisterContract>()))
                .ReturnsAsync(expectedPlayer);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = await controller.RegisterPlayerAsync(playerToRegister);

            // Assert
            var registeredPlayer = (result.Result as CreatedAtActionResult).Value as PlayerContract;

            registeredPlayer.Should().BeEquivalentTo(expectedPlayer);
            registeredPlayer.Id.Should().NotBeEmpty();
            registeredPlayer.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
