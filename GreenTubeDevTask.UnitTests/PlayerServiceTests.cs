using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryStub = new();
        private readonly Mock<IWalletService> _walletServiceStub = new();
        private readonly Mock<ILogger<PlayerService>> _loggerStub = new();

        [Fact]
        public async Task CreatePlayerAsync_WithPlayerToCreate_ReturnsCreatedPlayer()
        {
            // Arrange
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());

            _playerRepositoryStub.Setup(exp => exp.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((Player)null);
            _walletServiceStub.Setup(exp => exp.CreateWalletAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Wallet)null);
            var service = new PlayerService(_loggerStub.Object, _walletServiceStub.Object, _playerRepositoryStub.Object);

            // Act
            var result = await service.CreatePlayerAsync(playerToRegister);

            // Assert
            result.Should().BeEquivalentTo(playerToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task CreatePlayerAsync_WithExistingPlayerToCreate_ReturnsNull()
        {
            // Arrange
            var registeredPlayer = CreateRandomPlayer();
            var playerToRegister = new PlayerRegisterContract(Guid.NewGuid().ToString());
            _playerRepositoryStub.Setup(exp => exp.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(registeredPlayer);
            _walletServiceStub.Setup(exp => exp.CreateWalletAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Wallet)null);
            var service = new PlayerService(_loggerStub.Object, _walletServiceStub.Object, _playerRepositoryStub.Object);

            // Act
            var result = await service.CreatePlayerAsync(playerToRegister);

            // Assert
            result.Should().Be(null);
        }
    }
}
