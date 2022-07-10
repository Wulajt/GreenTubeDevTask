using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;

namespace GreenTubeDevTask.InMemRepositories
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        IEnumerable<Transaction> GetAllByPlayerId(Guid playerId);
    }
}