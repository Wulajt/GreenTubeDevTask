using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryStub = new();
        private readonly Mock<IWalletService> _walletServiceStub = new();
        private readonly Mock<ILogger<TransactionService>> _loggerStub = new();
        private readonly Random _rand = new();

        [Fact]
        public void RegisterTransaction_WithTransactionDeposit_ReturnsRegisteredTransaction()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Deposit, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(playerWallet);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void RegisterTransaction_WithTransactionDepositWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Deposit, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((Wallet)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionWin_ReturnsRegisteredTransaction()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Win, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(playerWallet);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void RegisterTransaction_WithTransactionWinWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Win, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((Wallet)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStake_ReturnsRegisteredTransaction()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Stake, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(playerWallet);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStakeWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Stake, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((Wallet)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStakeWithAmountGreaterThanBalance_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Stake, _rand.Next(1, 1000));
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((Wallet)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }
    }
}
