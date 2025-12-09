using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public enum TourTransportType
    {
        Foot,
        Bike,
        Car
    }

    public class TourTransportDurationDto
    {
        public double Duration { get; set; }
        public TourTransportType Transport { get; set; }
    }
}
