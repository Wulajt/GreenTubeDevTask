using GreenTubeDevTask.Entities;
using GreenTubeDevTask.Services;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        IEnumerable<Transaction> GetAllByPlayerId(Guid playerId);
        Transaction GetTransactionByPlayerIdAndIdempotentKey(Guid playerId, Guid idempotentKey);
    }
}