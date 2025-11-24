using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Options;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Rating : Entity
    {
        public long UserId { get; private set; }
        public int Value { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Rating(long userId, int value, string? comment)
        {
            UserId = userId;
            Value = value;
            Comment = comment;
            CreatedAt = DateTime.UtcNow;

            Validate();

        }

        public void Update(int value, string? comment)
        {
            Value = value;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;

            Validate();
        }

        private void Validate()
        {
            //if (UserId <= 0) throw new ArgumentException("Invalid UserId");
            if (Value < 1 || Value > 5) throw new ArgumentException("Rating must be between 1 and 5");
        }
    }
}
