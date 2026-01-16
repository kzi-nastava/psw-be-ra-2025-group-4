namespace Explorer.Payments.API.Dtos
{
    public class PaymentTourInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AuthorId { get; set; }
        public decimal Price { get; set; }
    }
}