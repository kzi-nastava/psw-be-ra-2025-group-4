using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourProblem : Entity
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public ProblemCategory Category { get; set; }
        public ProblemPriority Priority { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }

        public TourProblem(int tourId, ProblemCategory category, ProblemPriority priority, string description, DateTime time, int touristId)
        {
            TourId = tourId;
            Category = category;
            Priority = priority;
            Description = description;
            Time = time;
            TouristId = touristId;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty");
            if (TourId == 0)
                throw new ArgumentException("TourId must not be zero");
            if (TouristId == 0)
                throw new ArgumentException("TouristId must not be zero");
        }
    }

    public enum ProblemCategory
    {
        Booking,
        Itinerary,
        Guide,
        Transportation,
        Accommodation,
        Other
    }

    public enum ProblemPriority
    {
        Low,
        Medium,
        High
    }
}