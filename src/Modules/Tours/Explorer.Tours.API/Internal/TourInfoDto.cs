using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Internal
{
    public class TourInfoDto
    {
        public int TourId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public TourLifecycleStatus Status { get; set; }
    }

    public enum TourLifecycleStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }
}

