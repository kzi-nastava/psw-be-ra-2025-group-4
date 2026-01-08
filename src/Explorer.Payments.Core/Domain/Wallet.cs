using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Wallet : AggregateRoot
    {
        public int TouristId { get; private set; }
        public decimal Balance { get; private set; }

        private Wallet() { }

        public Wallet(int touristId)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.", nameof(touristId));
            TouristId = touristId;
            Balance = 0.0m;
        }

        public void AddBalance(decimal amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.", nameof(amount));
            if (amount > 0)
            {
                Balance += amount;
            }
        }

        public void DeductBalance(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (Balance < amount) throw new InvalidOperationException("Insufficient balance.");
            Balance -= amount;
        }
    }
}

