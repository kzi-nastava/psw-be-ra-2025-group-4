using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourSearchRequestDto
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double RadiusKm { get; set; }
    }
}
