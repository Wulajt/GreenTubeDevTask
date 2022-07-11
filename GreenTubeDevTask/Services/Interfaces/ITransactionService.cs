using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync();
        Task<Transaction> GetTransactionAsync(Guid id);
        Task<IEnumerable<Transaction>> GetTransactionsByPlayerIdAsync(Guid playerId);
#nullable enable
        Task<Transaction?> RegisterTransactionAsync(RegisterTransactionContract contract);
    }
}
