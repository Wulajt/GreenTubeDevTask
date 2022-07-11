using FluentAssertions;
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
    public class WalletsControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceStub = new();
        private readonly Mock<IWalletService> _walletServiceStub = new();
        private readonly Mock<ILogger<WalletsController>> _loggerStub = new();

        [Fact]
        public async Task GetWalletsAsync_WithExistingWallets_ReturnsAllWallets()
        {
            // Arrange
            var expectedWallets = new[] { CreateRandomWallet(), CreateRandomWallet(), CreateRandomWallet() };
            _walletServiceStub.Setup(exp => exp.GetWalletsAsync())
                .ReturnsAsync(expectedWallets);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.GetWalletsAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedWallets);
        }

        [Fact]
        public async Task GetWalletAsync_WithExistingWallet_ReturnsExpectedWallet()
        {
            // Arrange
            var expectedWallet = CreateRandomWallet();
            _walletServiceStub.Setup(exp => exp.GetWalletAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedWallet);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.GetWalletAsync(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedWallet);
        }

        [Fact]
        public async Task GetWalletAsync_WithNonExistingWallet_Returns()
        {
            // Arrange
            _walletServiceStub.Setup(exp => exp.GetWalletAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Wallet)null);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.GetWalletAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
