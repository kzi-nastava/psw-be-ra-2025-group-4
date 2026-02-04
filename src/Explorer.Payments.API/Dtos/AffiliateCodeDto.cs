using System;

namespace Explorer.Payments.API.Dtos
{
    public class AffiliateCodeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public int? TourId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; }
        public int AffiliateTouristId { get; set; }
        public decimal Percent { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int UsageCount { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string? TourName { get; set; }

    }
}
