﻿using System;

namespace GreenTubeDevTask.Entities
{
    public enum TransactionType
    {
        Stake = -1,
        Deposit,
        Win,
    }
    public class Transaction : IEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
