namespace Explorer.Payments.API.Dtos
{
    public class AffiliateTourStatsDto
    {
        public int TourId { get; set; }
        public int Usages { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
    }
}
