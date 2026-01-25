using System;

namespace Explorer.Tours.API.Dtos
{
    public class FavoriteTourDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
