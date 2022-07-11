using FluentAssertions;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task CreateWalletAsync_WithPlayerIdToCreate_ReturnsCreatedWallet()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.CreateWalletAsync(playerId);

            // Assert
            result.Id.Should().Be(playerId);
            result.Balance.Should().Be(decimal.Zero);
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task IncreaseWalletBalanceAsync_WithAmountToIncrease_ReturnsTrue()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            decimal amountToIncrease = _rand.Next(1, 1000);
            var expectedBalance = decimal.Add(playerWallet.Balance, amountToIncrease);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.IncreaseWalletBalanceAsync(Guid.NewGuid(), amountToIncrease);
            
            // Assert
            result.Should().Be(true);
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task IncreaseWalletBalanceAsync_WithNonExistingWallet_ReturnsNull()
        {
            // Arrange
            decimal amountToIncrease = _rand.Next(1, 1000);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Wallet)null);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.IncreaseWalletBalanceAsync(Guid.NewGuid(), amountToIncrease);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public async Task DecreaseWalletBalanceAsync_WithAmountToDecrease_ReturnsTrue()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1001, 10000));
            decimal amountToDecrease = _rand.Next(1, 1000);
            var expectedBalance = decimal.Subtract(playerWallet.Balance, amountToDecrease);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().Be(true);
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task DecreaseWalletBalanceAsync_WithAmountToDecreaseGreaterThenBalance_ReturnsFalse()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1, 1));
            decimal amountToDecrease = _rand.Next(1001, 10000);
            var expectedBalance = playerWallet.Balance;
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().Be(false);
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task DecreaseWalletBalanceAsync_WithNonExistingWallet_ReturnsNull()
        {
            // Arrange
            decimal amountToDecrease = _rand.Next(1001, 10000);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Wallet)null);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var result = await service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease);

            // Assert
            result.Should().Be(null);
        }

        //[Fact]
        //public void DecreaseWalletBalance_WithAmountToDecreaseFromMultipleThreads_ReturnsTrueConsistentBalance()
        //{
        //    // Arrange
        //    var playerWallet = CreateRandomWallet(_rand.Next(1001, 10000));
        //    decimal amountToDecrease = _rand.Next(1, 1000);
        //    var expectedBalance = decimal.Subtract(playerWallet.Balance, amountToDecrease);
        //    _walletRepositoryStub.Setup(exp => exp.GetById(It.IsAny<Guid>()))
        //        .Returns(playerWallet);
        //    var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

        //    // Act
        //    var result = service.DecreaseWalletBalance(Guid.NewGuid(), amountToDecrease);

        //    // Assert
        //    result.Should().Be(true);
        //    var updatedWallet = service.GetWallet(playerWallet.Id);
        //    updatedWallet.Balance.Should().Be(expectedBalance);
        //}

        //private void CallFromThread(Thread thread, int sleepTime)
        //{

        //}
    }
}
