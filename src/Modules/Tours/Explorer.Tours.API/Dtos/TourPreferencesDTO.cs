using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourPreferencesDTO
    {
        public long Id { get; set; }   

        public string PreferredDifficulty { get; set; } = string.Empty;

        public int WalkRating { get; set; }
        public int BikeRating { get; set; }
        public int CarRating { get; set; }
        public int BoatRating { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
