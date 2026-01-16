using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public enum BundleDtoStatus
    {
        Draft,
        Published,
        Archived
    }

    public class BundleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
        public BundleDtoStatus Status { get; set; }
        public List<TourDto> Tours { get; set; } = new List<TourDto>();
        public decimal TotalToursPrice { get; set; }
    }

    public class CreateBundleDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<int> TourIds { get; set; } = new List<int>();
    }

    public class UpdateBundleDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<int> TourIds { get; set; } = new List<int>();
    }
}

