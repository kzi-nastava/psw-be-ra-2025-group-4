using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class TourReviewDTO
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public List<string> Images { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TourCompletionPercentage { get; set; }

        public string TouristUsername { get; set; }
    }

        public class ReviewEligibilityInfo
        {
            public bool CanLeaveReview { get; set; }
            public string Reason { get; set; }
            public double CompletionPercentage { get; set; }
            public double DaysSinceLastActivity { get; set; }
        }
    
}