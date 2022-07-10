using FluentAssertions;
using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void RegisterTransaction_WithTransactionDeposit_ReturnsRegisteredTransactionAccepted()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Deposit, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(true);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            result.Status.Should().Be(TransactionStatus.Accepted);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionDepositWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Deposit, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((bool?)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionWin_ReturnsRegisteredTransactionAccepted()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Win, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(true);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            result.Status.Should().Be(TransactionStatus.Accepted);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionWinWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Win, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((bool?)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStake_ReturnsRegisteredTransactionAccepted()
        {
            // Arrange
            var playerWallet = CreateRandomWallet();
            var transactionToRegister = new RegisterTransactionContract(playerWallet.Id, TransactionType.Stake, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(true);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            result.Status.Should().Be(TransactionStatus.Accepted);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStakeWithNonExistingPlayer_ReturnsNull()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Stake, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns((bool?)null);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void RegisterTransaction_WithTransactionStakeWithAmountGreaterThanBalance_ReturnsTransactionRejected()
        {
            // Arrange
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Stake, _rand.Next(1, 1000), Guid.NewGuid());
            _walletServiceStub.Setup(exp => exp.DecreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(false);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);

            // Action
            var result = service.RegisterTransaction(transactionToRegister);

            // Assert
            result.Should().BeEquivalentTo(transactionToRegister);
            result.Id.Should().NotBeEmpty();
            result.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            result.Status.Should().Be(TransactionStatus.Rejected);
        }

        [Fact]
        public void RegisterTransaction_WithDepositMultipleSameRequests_ReturnsAcceptedIdempotentBehavior()
        {
            // Arrange
            decimal amountToDeposit = _rand.Next(1, 1000);
            var transactionToRegister = new RegisterTransactionContract(Guid.NewGuid(), TransactionType.Deposit, amountToDeposit, Guid.NewGuid());

            var numberOfCalls = _rand.Next(5, 15);
            _walletServiceStub.Setup(exp => exp.IncreaseWalletBalance(It.IsAny<Guid>(), It.IsAny<decimal>()))
                .Returns(true);
            var service = new TransactionService(_loggerStub.Object, _walletServiceStub.Object, _transactionRepositoryStub.Object);
            var firstResult = service.RegisterTransaction(transactionToRegister);
            _transactionRepositoryStub.Setup(exp => exp.GetTransactionByPlayerIdAndIdempotentKey(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(firstResult);

            // Action
            var results = new List<Transaction?>();
            for (int i = 0; i<numberOfCalls-1; i++)
            {
                results.Add(service.RegisterTransaction(transactionToRegister));
            }

            // Assert
            firstResult.Should().BeEquivalentTo(transactionToRegister);
            firstResult.Id.Should().NotBeEmpty();
            firstResult.DateCreated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            firstResult.Status.Should().Be(TransactionStatus.Accepted);
            foreach (var result in results)
            {
                result.Should().BeEquivalentTo(firstResult);
            }
        }
    }
}
