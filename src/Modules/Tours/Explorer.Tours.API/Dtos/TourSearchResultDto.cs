using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourSearchResultDto
    {
        public int TourId { get; set; }
        public string Name { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public MatchingPointDto MatchingPoint { get; set; } = new();
    }

    public class MatchingPointDto
    {
        public int KeyPointId { get; set; }
        public string Title { get; set; } = "";
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double DistanceKm { get; set; }
    }
}