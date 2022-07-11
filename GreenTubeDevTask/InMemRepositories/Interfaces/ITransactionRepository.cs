using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        Task<IEnumerable<Transaction>> GetAllByPlayerIdAsync(Guid playerId);
        Task<Transaction> GetTransactionByPlayerIdAndIdempotentKeyAsync(Guid playerId, Guid idempotentKey);
    }
}