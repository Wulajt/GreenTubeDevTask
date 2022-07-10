using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Controllers;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class PlayersControllerTests
    {
        private readonly Mock<IPlayerService> _playerServiceStub = new();
        private readonly Mock<ILogger<PlayersController>> _loggerStub = new();

        [Fact]
        public void GetPlayers_WithExistingPlayers_ReturnsAllPlayers()
        {
            // Arrange
            var expectedPlayers = new[] { CreateRandomPlayer(), CreateRandomPlayer(), CreateRandomPlayer() };
            _playerServiceStub.Setup(exp => exp.GetPlayers())
                .Returns(expectedPlayers);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = controller.GetPlayers();

            // Assert
            result.Should().BeEquivalentTo(expectedPlayers);
        }

        [Fact]
        public void GetPlayer_WithNonExistingPlayer_ReturnsNotFound()
        {
            // Arrange
            _playerServiceStub.Setup(exp => exp.GetPlayer(It.IsAny<Guid>()))
                .Returns((Player)null);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = controller.GetPlayer(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void GetPlayer_WithExistingPlayer_ReturnsExpectedPlayer()
        {
            // Arrange
            var expectedPlayer = CreateRandomPlayer();
            _playerServiceStub.Setup(exp => exp.GetPlayer(It.IsAny<Guid>()))
                .Returns(expectedPlayer);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = controller.GetPlayer(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedPlayer);
        }

        [Fact]
        public void RegisterPlayer_WithPlayerToRegisterWithExistingUsernamePlayerToRegister_ReturnsValidationProblem()
        {
            // Arrange
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());
            _playerServiceStub.Setup(exp => exp.CreatePlayer(It.IsAny<PlayerRegisterContract>()))
                .Returns((Player)null);

            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var registerResult = controller.RegisterPlayer(playerToRegister);

            // Assert
            var badRequestResult = registerResult.Result.Should().BeOfType<ObjectResult>().Which;
            var problemDetails = badRequestResult.Value.Should().BeOfType<ValidationProblemDetails>().Which;
            problemDetails.Detail.Should().Be("Username already registered.");
        }

        [Fact]
        public void RegisterPlayer_WithPlayerToRegister_ReturnsRegisteredPlayer()
        {
            // Arrange
            var expectedPlayer = CreateRandomPlayer();
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());
            _playerServiceStub.Setup(exp => exp.CreatePlayer(It.IsAny<PlayerRegisterContract>()))
                .Returns(expectedPlayer);
            var controller = new PlayersController(_loggerStub.Object, _playerServiceStub.Object);

            // Act
            var result = controller.RegisterPlayer(playerToRegister);

            // Assert
            var registeredPlayer = (result.Result as CreatedAtActionResult).Value as PlayerContract;

            registeredPlayer.Should().BeEquivalentTo(expectedPlayer);
            registeredPlayer.Id.Should().NotBeEmpty();
            registeredPlayer.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
