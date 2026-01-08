using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class MysteryTourOfferDto
    {
        public Guid Id { get; set; }
        public int TouristId { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = "";
        public decimal OriginalPrice { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountedPrice { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Redeemed { get; set; }
    }
}
