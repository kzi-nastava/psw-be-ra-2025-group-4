using System;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Payments.Core.Domain
{
    public class Coupon : AggregateRoot
    {
        public string Code { get; private set; }
        public int DiscountPercentage { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public int AuthorId { get; private set; }
        public int? TourId { get; private set; } 
        public bool IsUsed { get; private set; }
        public int? UsedByTouristId { get; private set; }
        public DateTime? UsedAt { get; private set; }

        private Coupon() { }

        public Coupon(string code, int discountPercentage, int authorId, DateTime? expirationDate = null, int? tourId = null)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 8)
                throw new EntityValidationException("Coupon code must be exactly 8 characters.");

            if (discountPercentage <= 0 || discountPercentage > 100)
                throw new EntityValidationException("Discount percentage must be between 1 and 100.");

            if (authorId == 0)
                throw new EntityValidationException("Invalid author id.");

            Code = code.ToUpper();
            DiscountPercentage = discountPercentage;
            AuthorId = authorId;
            ExpirationDate = expirationDate;
            TourId = tourId;
            IsUsed = false;
        }

        public void Update(int discountPercentage, DateTime? expirationDate, int? tourId)
        {
            if (IsUsed)
                throw new InvalidOperationException("Cannot update a used coupon.");

            if (discountPercentage <= 0 || discountPercentage > 100)
                throw new EntityValidationException("Discount percentage must be between 1 and 100.");

            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            TourId = tourId;
        }

        public void MarkAsUsed(int touristId)
        {
            if (IsUsed)
                throw new InvalidOperationException("Coupon has already been used.");

            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Coupon has expired.");

            IsUsed = true;
            UsedByTouristId = touristId;
            UsedAt = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            if (IsUsed) return false;
            if (ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow) return false;
            return true;
        }

        public static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new char[8];

            for (int i = 0; i < 8; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }
    }
}