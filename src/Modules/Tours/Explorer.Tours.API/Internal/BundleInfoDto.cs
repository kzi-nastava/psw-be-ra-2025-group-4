using System.Collections.Generic;

namespace Explorer.Tours.API.Internal
{
    public class BundleInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public BundleLifecycleStatus Status { get; set; }
        public List<BundleTourInfoDto> Tours { get; set; } = new List<BundleTourInfoDto>();
    }

    public class BundleTourInfoDto
    {
        public int Id { get; set; }
    }

    public enum BundleLifecycleStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }
}

