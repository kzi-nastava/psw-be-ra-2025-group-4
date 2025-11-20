using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourProblemDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public ProblemCategoryDto Category { get; set; }
        public ProblemPriorityDto Priority { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
    }

    public enum ProblemCategoryDto
    {
        Booking,
        Itinerary,
        Guide,
        Transportation,
        Accommodation,
        Other
    }

    public enum ProblemPriorityDto
    {
        Low,
        Medium,
        High
    }
}