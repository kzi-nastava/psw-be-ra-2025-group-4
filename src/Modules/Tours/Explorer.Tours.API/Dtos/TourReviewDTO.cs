using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourReviewCreateDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class TourReviewDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public int TourId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public double CompletionPercentage { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public class TourReviewResponseDto
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public double CompletionPercentage { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public class ReviewEligibilityDto
    {
        public bool CanLeaveReview { get; set; }
        public string Reason { get; set; }
        public double CompletionPercentage { get; set; }
        public int DaysSinceLastActivity { get; set; }
        public TourReviewResponseDto ExistingReview { get; set; }
    }
}