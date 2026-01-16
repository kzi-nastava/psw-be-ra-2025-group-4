namespace Explorer.Payments.API.Dtos
{
    public class CouponUpdateDto
    {
        public int Id { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? TourId { get; set; }
    }
}