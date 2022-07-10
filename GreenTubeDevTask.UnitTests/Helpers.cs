using GreenTubeDevTask.Entities;
using System;

namespace GreenTubeDevTask.UnitTests
{
    public static class Helpers
    {
        private static Random _rand = new();

        public static Player CreateRandomPlayer()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Username = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
            };
        }

        public static Wallet CreateRandomWallet()
        {
            return CreateRandomWallet(_rand.Next(1, 1000));
        }

        public static Wallet CreateRandomWallet(decimal amount)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Balance = amount,
                DateCreated = DateTime.Now
            };
        }
    }
}
