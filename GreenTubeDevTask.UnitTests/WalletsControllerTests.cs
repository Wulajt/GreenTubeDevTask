using FluentAssertions;
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
    public class WalletsControllerTests
    {
        private readonly Mock<IWalletService> _walletServiceStub = new();
        private readonly Mock<ILogger<WalletsController>> _loggerStub = new();

        [Fact]
        public void GetWallets_WithExistingWallets_ReturnsAllWallets()
        {
            // Arrange
            var expectedWallets = new[] { CreateRandomWallet(), CreateRandomWallet(), CreateRandomWallet() };
            _walletServiceStub.Setup(exp => exp.GetWallets())
                .Returns(expectedWallets);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object);

            // Act
            var result = controller.GetWallets();

            // Assert
            result.Should().BeEquivalentTo(expectedWallets);
        }

        [Fact]
        public void GetWallet_WithExistingWallet_ReturnsExpectedWallet()
        {
            // Arrange
            var expectedWallet = CreateRandomWallet();
            _walletServiceStub.Setup(exp => exp.GetWallet(It.IsAny<Guid>()))
                .Returns(expectedWallet);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object);

            // Act
            var result = controller.GetWallet(Guid.NewGuid());

            // Assert
            result.Value.Should().BeEquivalentTo(expectedWallet);
        }

        [Fact]
        public void GetWallet_WithNonExistingWallet_Returns()
        {
            // Arrange
            _walletServiceStub.Setup(exp => exp.GetWallet(It.IsAny<Guid>()))
                .Returns((Wallet)null);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object);

            // Act
            var result = controller.GetWallet(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
