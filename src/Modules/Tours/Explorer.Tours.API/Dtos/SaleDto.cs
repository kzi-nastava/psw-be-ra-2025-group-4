using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public List<int> TourIds { get; set; } = new List<int>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrentlyActive { get; set; }
    }

    public class SaleCreateDto
    {
        public List<int> TourIds { get; set; } = new List<int>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercent { get; set; }
    }

    public class SaleUpdateDto
    {
        public List<int> TourIds { get; set; } = new List<int>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountPercent { get; set; }
    }
}

