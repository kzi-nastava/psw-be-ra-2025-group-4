namespace Explorer.Payments.API.Dtos
{
    public class CouponValidationDto
    {
        public string Code { get; set; }
        public int TouristId { get; set; }
        public List<int> TourIds { get; set; } 
    }
}