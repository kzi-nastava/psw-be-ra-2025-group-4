using System;

namespace Explorer.Payments.API.Dtos
{
    public class CoinsBundleSaleDto
    {
        public int Id { get; set; }
        public int CoinsBundleId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}