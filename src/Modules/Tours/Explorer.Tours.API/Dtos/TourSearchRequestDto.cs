namespace Explorer.Tours.API.Dtos
{
    public class TourSearchRequestDto
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double RadiusKm { get; set; }
    }
}