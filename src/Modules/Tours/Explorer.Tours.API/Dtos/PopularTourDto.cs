using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PopularTourDto
    {
        public int TourId { get; set; }
        public string Name { get; set; } = "";
        public TourDtoStatus Status { get; set; }

        public double Popularity { get; set; }    
        public int RatingsCount { get; set; }      

       
        public double? DistanceKm { get; set; }   
    }
}
