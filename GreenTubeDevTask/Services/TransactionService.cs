using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public class TransactionService : ITransactionService
    {
        // Public
        public TransactionService(ILogger<TransactionService> logger, IWalletService walletService, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            _walletService = walletService;
            _repository = transactionRepository;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<IEnumerable<Transaction>> GetTransactionsByPlayerIdAsync(Guid playerId)
        {
            return await _repository.GetAllByPlayerIdAsync(playerId);
        }
        public async Task<Transaction> GetTransactionAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }
#nullable enable
        public async Task<Transaction?> RegisterTransactionAsync(RegisterTransactionContract contract)
        {
            mutexLock.WaitOne();
            Transaction? result = null;
            var transactionExists = await _repository.GetTransactionByPlayerIdAndIdempotentKeyAsync(contract.PlayerId, contract.IdempotentKey);
            if (transactionExists is null)
            {
                bool? updateResult;
                switch (contract.Type)
                {
                    case TransactionType.Win:
                        updateResult = await _walletService.IncreaseWalletBalanceAsync(contract.PlayerId, contract.Amount);
                        break;

                    case TransactionType.Deposit:
                        updateResult = await _walletService.IncreaseWalletBalanceAsync(contract.PlayerId, contract.Amount);
                        break;

                    case TransactionType.Stake:
                        updateResult = await _walletService.DecreaseWalletBalanceAsync(contract.PlayerId, contract.Amount);
                        break;

                    default:
                        updateResult = null;
                        break;
                }
                if (updateResult is not null)
                {
                    var newTransaction = GenerateTransaction(contract);
                    if ((bool)updateResult)
                        newTransaction.Status = TransactionStatus.Accepted;
                    else
                        newTransaction.Status = TransactionStatus.Rejected;
                    await _repository.AddAsync(newTransaction);
                    result = newTransaction;
                }
                else
                    result = null;
            }
            else
                result = transactionExists;
            mutexLock.ReleaseMutex();
            return await Task.FromResult(result);
        }
#nullable disable
        // Private
        private Transaction GenerateTransaction(RegisterTransactionContract contract)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                PlayerId = contract.PlayerId,
                Type = contract.Type,
                Amount = contract.Amount,
                DateCreated = DateTime.Now,
                IdempotentKey = contract.IdempotentKey,
                Status = TransactionStatus.Default
            };
        }
        private readonly ILogger<TransactionService> _logger;
        private readonly ITransactionRepository _repository;
        private readonly IWalletService _walletService;
        private static Mutex mutexLock = new Mutex(false);
    }
}
