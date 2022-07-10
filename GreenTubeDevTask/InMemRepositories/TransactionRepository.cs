using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenTubeDevTask.InMemRepositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository, IRepositoryBase<Transaction>
    {
        public IEnumerable<Transaction> GetAllByPlayerId(Guid playerId)
        {
            return GetAll().Where(transaction => transaction.PlayerId == playerId);
        }
    }
}
