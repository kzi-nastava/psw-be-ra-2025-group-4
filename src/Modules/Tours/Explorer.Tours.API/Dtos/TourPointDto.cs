namespace Explorer.Tours.API.Dtos
{
    public class TourPointDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Order { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageBase64 { get; set; }
        public string? Secret { get; set; }
    }
}
