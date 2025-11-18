using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourProblem : Entity
    {
        public int TourProblemId { get; set; }
        public int TourId { get; set; }
        public ProblemCategory Category { get; set; }
        public ProblemPriority Priority { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
        public int TouristId { get; set; }

        public TourProblem(int tourId, ProblemCategory category, ProblemPriority priority, string description, DateTime time, int touristId)
        {
            if (tourId <= 0) throw new ArgumentException("Invalid TourId.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");

            TourId = tourId;
            Category = category;
            Priority = priority;
            Description = description;
            Time = time;
            TouristId = touristId;
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
