namespace Explorer.Payments.API.Internal
{
    public interface ITourLookupService
    {
        TourLookupDto? Get(int tourId);
    }

    public class TourLookupDto
    {
        public int TourId { get; set; }
        public string? Name { get; set; }
        public int AuthorId { get; set; }
    }
}
