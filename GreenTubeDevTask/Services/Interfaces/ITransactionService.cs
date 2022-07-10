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
        IEnumerable<Transaction> GetTransactions();
        Transaction GetTransaction(Guid id);
        IEnumerable<Transaction> GetTransactionsByPlayerId(Guid playerId);
#nullable enable
        Transaction? RegisterTransaction(RegisterTransactionContract contract);
    }
}
