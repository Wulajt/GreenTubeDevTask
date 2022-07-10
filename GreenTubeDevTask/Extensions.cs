using GreenTubeDevTask.Contracts;
using GreenTubeDevTask.Entities;

namespace GreenTubeDevTask
{
    public static class Extensions
    {
        public static PlayerContract AsContract(this Player player)
        {
            return new PlayerContract(player.Id, player.Username, player.DateCreated);
        }

        public static WalletContract AsContract(this Wallet wallet)
        {
            return new WalletContract(wallet.Id, wallet.Balance, wallet.DateCreated);
        }

        public static TransactionContract AsContract(this Transaction transaction)
        {
            return new TransactionContract(transaction.Id, transaction.PlayerId, transaction.Type, transaction.Amount, transaction.DateCreated, transaction.Status);
        }
    }
}
