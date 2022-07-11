using GreenTubeDevTask.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenTubeDevTask.InMemRepositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository, IRepositoryBase<Transaction>
    {
        public async Task<IEnumerable<Transaction>> GetAllByPlayerIdAsync(Guid playerId)
        {
            return (await GetAllAsync()).Where(transaction => transaction.PlayerId == playerId);
        }

        async Task<Transaction> ITransactionRepository.GetTransactionByPlayerIdAndIdempotentKeyAsync(Guid playerId, Guid idempotentKey)
        {
            return (await GetAllAsync())
                .Where(transaction => (transaction.PlayerId == playerId && transaction.IdempotentKey == idempotentKey))
                .FirstOrDefault();
        }
    }
}
