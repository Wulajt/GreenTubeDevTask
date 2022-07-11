using FluentAssertions;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using GreenTubeDevTask.Services;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static GreenTubeDevTask.UnitTests.Helpers;

namespace GreenTubeDevTask.UnitTests
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryStub = new();
        private readonly Mock<ILogger<WalletService>> _loggerStub = new();
        private readonly Random _rand = new();

        private readonly ITestOutputHelper output;

        public WalletServiceTests(ITestOutputHelper output)
        {
            this.output = output;
        }

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

        [Fact]
        public async Task IncreaseWalletBalanceAsync_WithAmountToIncreaseFromMultipleThreads_ReturnsTrueConsistentBalance()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1, 1000));
            decimal amountToDecrease = _rand.Next(1, 1000);
            var numberOfCalls = _rand.Next(5, 15);
            decimal expectedIncrease = decimal.Multiply(amountToDecrease, numberOfCalls);
            var expectedBalance = decimal.Add(playerWallet.Balance, expectedIncrease);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var tasks = new List<Task<bool?>>();
            Parallel.For(0, numberOfCalls,
                index =>
                {
                    tasks.Add(service.IncreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease));
                });
            Task.WaitAll(tasks.ToArray());

            // Assert
            foreach (var task in tasks)
            {
                task.Result.Should().Be(true);
            }
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        // Sometimes it fails because of Parallel for loop because of writing counts at same times, needs to be fixed
        [Fact]
        public async Task DecreaseWalletBalanceAsync_WithAmountToDecreaseFromMultipleThreadsAddingToGreaterAmountThenBalance_ReturnsTrueOrFalseConsistentBalance()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(1000, 2000));
            decimal amountToDecrease = _rand.Next(500, 1000);
            var numberOfCalls = _rand.Next(5, 15);

            decimal virtualBalance = playerWallet.Balance;
            int numberOfAcceptedDecreases = 0;
            while (virtualBalance >= decimal.Zero)
            {
                var updatedVirtualBalance = decimal.Subtract(virtualBalance, amountToDecrease);
                if (updatedVirtualBalance >= decimal.Zero)
                {
                    virtualBalance = updatedVirtualBalance;
                    numberOfAcceptedDecreases++;
                }
                else
                    break;
            }
            output.WriteLine("StartBallance {0}", playerWallet.Balance);
            output.WriteLine("AmountToDecrease {0}", amountToDecrease);
            output.WriteLine("Calls {0}", numberOfCalls);
            output.WriteLine("Decreases {0}", numberOfAcceptedDecreases);

            //int numberOfAcceptedDecreases = (int) (decimal.Divide(playerWallet.Balance, amountToDecrease));
            decimal expectedDecrease = decimal.Multiply(amountToDecrease, numberOfAcceptedDecreases);
            var expectedBalance = decimal.Subtract(playerWallet.Balance, expectedDecrease);

            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var actualCalls = 0;
            int acceptedTasks = 0;
            int rejectedTasks = 0;
            Parallel.For(0, numberOfCalls,
                async index =>
                {
                    var result = await service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease);
                    output.WriteLine("Task {0}", result);
                    if ((bool)result)
                        acceptedTasks++;
                    else
                        rejectedTasks++;
                    actualCalls++;
                });
            output.WriteLine("ActualCalls {0}", actualCalls);

            // Assert
            acceptedTasks.Should().Be(numberOfAcceptedDecreases);
            rejectedTasks.Should().Be(numberOfCalls - numberOfAcceptedDecreases);
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task DecreaseWalletBalanceAsync_WithAmountToDecreaseFromMultipleThreads_ReturnsTrueConsistentBalance()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(15001, 100000));
            decimal amountToDecrease = _rand.Next(1, 1000);
            var numberOfCalls = _rand.Next(5, 15);
            decimal expectedDecrease = decimal.Multiply(amountToDecrease, numberOfCalls);
            var expectedBalance = decimal.Subtract(playerWallet.Balance, expectedDecrease);
            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var tasks = new List<Task<bool?>>();
            Parallel.For(0, numberOfCalls,
                index =>
                {
                    tasks.Add(service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease));
                });
            Task.WaitAll(tasks.ToArray());

            // Assert
            foreach (var task in tasks)
            {
                task.Result.Should().Be(true);
            }
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }

        [Fact]
        public async Task IncreaseDecreaseWalletBalanceAsync_WithAmountToIncreaseDecreaseFromMultipleThreads_ReturnsTrueConsistentBalanceFromBothActions()
        {
            // Arrange
            var playerWallet = CreateRandomWallet(_rand.Next(10001, 100000));
            decimal amountToDecrease = _rand.Next(1, 1000);
            decimal amountToIncrease = _rand.Next(1, 1000);

            var numberOfIncreaseCalls = _rand.Next(1, 3);
            var numberOfDecreaseCalls = _rand.Next(1, 3);
            var numberOfCalls = numberOfIncreaseCalls + numberOfDecreaseCalls;

            decimal expectedIncrease = decimal.Multiply(amountToIncrease, numberOfIncreaseCalls);
            decimal expectedDecrease = decimal.Multiply(amountToDecrease, numberOfDecreaseCalls);
            decimal expectedBalance = decimal.Add(playerWallet.Balance, expectedIncrease);
            expectedBalance = decimal.Subtract(expectedBalance, expectedDecrease);

            _walletRepositoryStub.Setup(exp => exp.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(playerWallet);
            var service = new WalletService(_loggerStub.Object, _walletRepositoryStub.Object);

            // Act
            var tasks = new List<Task<bool?>>();
            for (int i = 0; i < numberOfCalls; i++)
            {
                if (numberOfIncreaseCalls > 0 && numberOfDecreaseCalls > 0)
                {
                    var randResult = _rand.Next(0, 1);
                    if (randResult == 1)
                    {
                        tasks.Add(service.IncreaseWalletBalanceAsync(Guid.NewGuid(), amountToIncrease));
                        numberOfIncreaseCalls--;
                    }
                    else
                    {
                        tasks.Add(service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease));
                        numberOfDecreaseCalls--;
                    }
                }
                else
                {
                    if (numberOfDecreaseCalls == 0)
                        tasks.Add(service.IncreaseWalletBalanceAsync(Guid.NewGuid(), amountToIncrease));
                    else
                        tasks.Add(service.DecreaseWalletBalanceAsync(Guid.NewGuid(), amountToDecrease));
                }
            }
            Task.WaitAll(tasks.ToArray());

            // Assert
            foreach (var task in tasks)
            {
                task.Result.Should().Be(true);
            }
            var updatedWallet = await service.GetWalletAsync(playerWallet.Id);
            updatedWallet.Balance.Should().Be(expectedBalance);
        }
    }
}
