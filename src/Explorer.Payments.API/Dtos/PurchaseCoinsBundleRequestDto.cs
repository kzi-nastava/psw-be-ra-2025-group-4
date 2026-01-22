namespace Explorer.Payments.API.Dtos
{
    public class PurchaseCoinsBundleRequestDto
    {
        public int CoinsBundleId { get; set; }
        public string PaymentMethod { get; set; } 

        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }

        public string? PayPalEmail { get; set; }

        public string? GiftCardCode { get; set; }
    }
}