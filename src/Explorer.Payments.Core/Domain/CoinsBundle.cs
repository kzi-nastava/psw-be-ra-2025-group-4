using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class CoinsBundle : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int CoinsAmount { get; private set; }
        public int BonusCoins { get; private set; }
        public decimal Price { get; private set; } // Cena u EUR
        public string ImageUrl { get; private set; }
        public int DisplayOrder { get; private set; } // Za sortiranje prikaza

        private CoinsBundle() { }

        public CoinsBundle(string name, string description, int coinsAmount, int bonusCoins,
            decimal price, string imageUrl, int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            if (coinsAmount <= 0)
                throw new ArgumentException("Coins amount must be positive.", nameof(coinsAmount));
            if (bonusCoins < 0)
                throw new ArgumentException("Bonus coins cannot be negative.", nameof(bonusCoins));
            if (price <= 0)
                throw new ArgumentException("Price must be positive.", nameof(price));

            Name = name;
            Description = description;
            CoinsAmount = coinsAmount;
            BonusCoins = bonusCoins;
            Price = price;
            ImageUrl = imageUrl ?? string.Empty;
            DisplayOrder = displayOrder;
        }

        public int GetTotalCoins() => CoinsAmount + BonusCoins;
    }
}