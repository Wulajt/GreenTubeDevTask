using FluentAssertions;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryStub = new();
        private readonly Mock<ILogger<WalletService>> _loggerStub = new();
        private readonly Random _rand = new();

        [Fact]
        public void CreateWallet_WithPlayerIdToCreate_ReturnsCreatedWallet()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.CreateWallet(playerId);

            // Assert
            result.Id.Should().Be(playerId);
            result.Balance.Should().Be(decimal.Zero);
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void IncreaseWalletBalance_WithAmountToIncrease_ReturnsUpdatedWallet()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            decimal amountToIncrease = _rand.Next(1, 1000);
            var expectedBalance = decimal.Add(playerWallet.Balance, amountToIncrease);
            _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
                .Returns(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.IncreaseWalletBalance(Guid.NewGuid(), amountToIncrease);
            
            // Assert
            result.Should().BeEquivalentTo(
                playerWallet,
                options => options.Excluding(x => x.Balance));
            result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public void IncreaseWalletBalance_WithNonExistingWallet_ReturnsUpdatedWallet()
        {
            // Arrange
            decimal amountToIncrease = _rand.Next(1, 1000);
            _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
                .Returns((Wallet)null);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.IncreaseWalletBalance(Guid.NewGuid(), amountToIncrease);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void DecreaseWalletBalance_WithAmountToDecrease_ReturnsUpdatedWallet()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1001, 10000));
            decimal amountToDecrease = _rand.Next(1, 1000);
            var expectedBalance = decimal.Subtract(playerWallet.Balance, amountToDecrease);
            _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
                .Returns(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.DecreaseWalletBalance(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().BeEquivalentTo(
                playerWallet,
                options => options.Excluding(x => x.Balance));
            result.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public void DecreaseWalletBalance_WithAmountToDecreaseGreaterThenBalance_ReturnsNull()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1, 1));
            decimal amountToDecrease = _rand.Next(1001, 10000);
            var expectedBalance = playerWallet.Balance;
            _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
                .Returns(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.DecreaseWalletBalance(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().Be(null);
            playerWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public void DecreaseWalletBalance_WithNonExistingWallet_ReturnsNull()
        {
            // Arrange
            decimal amountToDecrease = _rand.Next(1001, 10000);
            _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
                .Returns((Wallet)null);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = service.DecreaseWalletBalance(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().Be(null);
        }
    }
}
