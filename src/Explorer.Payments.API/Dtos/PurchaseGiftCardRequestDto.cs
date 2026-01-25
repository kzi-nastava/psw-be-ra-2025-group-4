namespace Explorer.Payments.API.Dtos
{
    public class PurchaseGiftCardRequestDto
    {
        public string RecipientUsername { get; set; } = "";
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = ""; // "CreditCard" | "PayPal"

        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }

        public string? PayPalEmail { get; set; }
    }
}
