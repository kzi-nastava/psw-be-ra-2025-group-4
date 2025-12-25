namespace Explorer.Payments.API.Dtos;

public class AddToCartRequestDto
{
    public int TourId { get; set; }
    public string TourName { get; set; } = "";
    public decimal Price { get; set; }
    public string Status { get; set; } = ""; // "Published" / "Archived"
}
