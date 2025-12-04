using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        [JsonConstructor]
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
            Validate();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public Money Add(Money money)
        {
            if(!IsSameCurrency(money))
                throw new ArgumentException("Can't add different currencies");

            return new Money(money.Amount + Amount, Currency);
        }



        public Money Subtract(Money money)
        {
            if (!IsSameCurrency(money))
                throw new ArgumentException("Can't subtract different currencies");

            if (money.Amount > Amount)
                throw new ArgumentException("Ammount can't be negative!");

            return new Money(Amount - money.Amount, Currency);
        }

        public bool IsSameCurrency(Money money)
        {
            return money.Currency == Currency;
        }

        private void Validate()
        {
            if (Amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(Currency))
                throw new ArgumentException("Currency cannot be empty.");
        }
    }
}
