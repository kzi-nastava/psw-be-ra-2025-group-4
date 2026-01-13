namespace Explorer.Payments.API.Dtos
{
    public class CouponResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int AuthorId { get; set; }
        public int? TourId { get; set; }
        public string TourName { get; set; } 
        public bool IsUsed { get; set; }
        public int? UsedByTouristId { get; set; }
        public DateTime? UsedAt { get; set; }
        public bool IsValid { get; set; }
    }
}