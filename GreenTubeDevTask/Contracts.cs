using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Contracts
{
    // Player Contracts
    public record PlayerContract(Guid Id, string Username, DateTime DateCreated);
    public record PlayerRegisterContract([Required] string Username);

    // Wallet Contracts
    public record WalletContract(Guid Id, decimal Balance, DateTime DateCreated);

    // Transaction Contracts
    public record TransactionContracts([Required] Guid Id, [Required] Guid PlayerId, [Required] TransactionType Type, [Required] decimal Amount);
}
