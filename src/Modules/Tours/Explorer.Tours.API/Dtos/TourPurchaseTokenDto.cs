using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourPurchaseTokenDto
    {
        public int Id { get; set; }
        public int TouristId { get; set; }
        public int TourId { get; set; }
        public DateTime PurchasedAt { get; set; }
    }
}