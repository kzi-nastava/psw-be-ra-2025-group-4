namespace Explorer.Payments.API.Dtos
{
    public class CouponValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int? ApplicableTourId { get; set; } 
        public decimal DiscountAmount { get; set; }
        public int DiscountPercentage { get; set; }
    }
}