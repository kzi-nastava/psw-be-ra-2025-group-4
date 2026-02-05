using System;

namespace Explorer.Payments.API.Dtos
{
    public class CoinsBundlePurchaseDto
    {
        public int Id { get; set; }
        public int TouristId { get; set; }
        public int CoinsBundleId { get; set; }
        public string BundleName { get; set; }
        public int CoinsReceived { get; set; }
        public decimal PricePaid { get; set; }
        public decimal OriginalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string TransactionId { get; set; }
    }
}