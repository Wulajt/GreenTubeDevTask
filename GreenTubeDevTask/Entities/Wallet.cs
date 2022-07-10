using System;

namespace GreenTubeDevTask.Entities
{
    public class Wallet : IEntity
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Balance { get; set; } = decimal.Zero;
    }
}
