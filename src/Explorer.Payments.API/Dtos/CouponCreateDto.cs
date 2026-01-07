using System;

namespace Explorer.Payments.API.Dtos
{
    public class CouponCreateDto
    {
        public int DiscountPercentage { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? TourId { get; set; }
    }
}