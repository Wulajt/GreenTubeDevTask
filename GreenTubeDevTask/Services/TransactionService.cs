using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using GreenTubeDevTask.InMemRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private readonly ITransactionRepository _repository;
        private readonly IWalletService _walletService;
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
            var transactionExists = await _repository.GetTransactionByPlayerIdAndIdempotentKeyAsync(contract.PlayerId, contract.IdempotentKey);
            if (transactionExists is not null) return transactionExists;

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
                    return null;
            }
            if (updateResult is null) return null;

            var newTransaction = GenerateTransaction(contract);
            if ((bool)updateResult)
                newTransaction.Status = TransactionStatus.Accepted;
            else
                newTransaction.Status = TransactionStatus.Rejected;
            await _repository.AddAsync(newTransaction);
            return await Task.FromResult(newTransaction);
        }
#nullable disable
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

        //public Transaction CreateTransaction(TransactionType type, decimal amount, Guid walletId)
        //{
        //    _logger.LogInformation("Creating Transaction, WalletId: {0}, Type: {1}, Amount: {2}.", walletId, type, amount);
        //    var wallet = _walletService.GetWallet(walletId);
        //    if (_repository.UsernameExists(player.Username))
        //    {
        //        _logger.LogInformation("Failed to create Player, username exists. Username: {0}", player.Username);
        //        return null;
        //    }

        //    Guid newPlayerId = Guid.NewGuid();
        //    var newWallet = _walletService.CreateWallet(newPlayerId);

        //    Player newPlayer = new()
        //    {
        //        Id = newPlayerId,
        //        WalletId = newWallet.Id,
        //        Username = player.Username,
        //        DateCreated = DateTime.Now
        //    };
        //    _repository.Add(newPlayer);
        //    _logger.LogInformation("Created Player: {0}.", newPlayer);
        //    return newPlayer;
        //}
    }
}
