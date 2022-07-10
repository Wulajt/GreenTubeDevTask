using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public class TransactionRepository : BaseRepository<Transaction>
    {
        public IEnumerable<Transaction> GetAllByPlayerId(Guid playerId)
        {
            return Get().Where(transaction => transaction.PlayerId == playerId);
        }
    }
}
