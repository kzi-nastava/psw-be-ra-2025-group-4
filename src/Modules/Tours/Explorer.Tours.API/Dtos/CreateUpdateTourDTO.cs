using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateUpdateTourDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TourDtoDifficulty Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public List<TourTransportDurationDto> TransportDuration { get; set; }
    }

}
