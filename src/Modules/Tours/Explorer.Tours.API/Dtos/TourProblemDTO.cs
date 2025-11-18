using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourProblemDto
    {
        public int TourProblemId { get; set; }
        public int TourId { get; set; }
        public ProblemCategoryDto Category { get; set; }
        public ProblemPriorityDto Priority { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }

        public int TouristId { get; set; }
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
