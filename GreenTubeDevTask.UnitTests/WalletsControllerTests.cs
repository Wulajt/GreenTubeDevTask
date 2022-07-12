using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Controllers;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Random _rand = new();

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
        public async Task GetWalletAsync_WithNonExistingWallet_ReturnsNotFound()
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

        [Fact]
        public async Task RegisterTransactionAsync_WithTransactionToRegisterDeposit_ReturnsRegisteredTransactionAccepted()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(
                Guid.NewGuid(),
                TransactionType.Deposit,
                _rand.Next(1000, 10000),
                Guid.NewGuid());
            var expectedTransaction = CreateRandomTransaction();
            _transactionServiceStub.Setup(exp => exp.RegisterTransactionAsync(It.IsAny<RegisterTransactionContract>()))
                .ReturnsAsync(expectedTransaction);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.RegisterTransactionAsync(transactionToRegister);

            // Assert
            var registeredTransaction = result.Value;

            registeredTransaction.Should().BeEquivalentTo(expectedTransaction,
                options => options.Excluding(x => x.IdempotentKey));
            registeredTransaction.Id.Should().NotBeEmpty();
            registeredTransaction.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            registeredTransaction.Status.Should().Be(TransactionStatus.Accepted);
        }

        [Fact]
        public async Task RegisterTransactionAsync_WithNonExistingWallet_ReturnsNotFound()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(
                Guid.NewGuid(),
                TransactionType.Deposit,
                _rand.Next(1000, 10000),
                Guid.NewGuid());
            _transactionServiceStub.Setup(exp => exp.RegisterTransactionAsync(It.IsAny<RegisterTransactionContract>()))
                .ReturnsAsync((Transaction?)null);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.RegisterTransactionAsync(transactionToRegister);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetWalletTransactionsAsync_WithExistingWallet_ReturnsAllWalletTrasnactions()
        {
            // Arrange
            var expectedTransactions = new[] { CreateRandomTransaction(), CreateRandomTransaction(), CreateRandomTransaction() };
            _transactionServiceStub.Setup(exp => exp.GetTransactionsByPlayerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedTransactions);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.GetWalletTransactionsAsync(Guid.NewGuid());

            // Assert
            var resultTransactions = (result.Result as OkObjectResult).Value;

            result.Result.Should().BeOfType<OkObjectResult>();
            resultTransactions.Should().BeEquivalentTo(expectedTransactions.Select(x => x.AsContract()));
        }

        [Fact]
        public async Task GetWalletTransactionsAsync_WithNonExistingWallet_ReturnsNotFound()
        {
            // Arrange
            var expectedTransactions = new[] { CreateRandomTransaction(), CreateRandomTransaction(), CreateRandomTransaction() };
            _transactionServiceStub.Setup(exp => exp.GetTransactionsByPlayerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((IEnumerable<Transaction>)null);
            var controller = new WalletsController(_loggerStub.Object, _walletServiceStub.Object, _transactionServiceStub.Object);

            // Act
            var result = await controller.GetWalletTransactionsAsync(Guid.NewGuid());

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
