using GreenTubeDevTask.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace GreenTubeDevTask.Contracts
{
    // Player Contracts
    public record PlayerContract(Guid Id, string Username, DateTime DateCreated);
    public record PlayerRegisterContract([Required] string Username);

    // Wallet Contracts
    public record WalletContract(Guid Id, decimal Balance, DateTime DateCreated);

    // Transaction Contracts
    public record TransactionContract(Guid Id, Guid PlayerId, TransactionType Type, decimal Amount, DateTime DateCreated);
    public record RegisterTransactionContract([Required] Guid PlayerId, [Required] TransactionType Type, [Required] decimal Amount);
}
