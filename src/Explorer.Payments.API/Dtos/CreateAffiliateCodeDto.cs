namespace Explorer.Payments.API.Dtos
{
    public class CreateAffiliateCodeDto
    {
        public int? TourId { get; set; }
        public int AffiliateTouristId { get; set; }
        public decimal Percent { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
