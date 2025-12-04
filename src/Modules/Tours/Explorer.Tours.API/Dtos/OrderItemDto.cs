using System;

namespace Explorer.Tours.API.Dtos
{
    public class OrderItemDto
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public decimal Price { get; set; }
    }
}
