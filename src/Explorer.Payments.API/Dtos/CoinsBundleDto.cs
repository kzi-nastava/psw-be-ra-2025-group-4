namespace Explorer.Payments.API.Dtos
{
    public class CoinsBundleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CoinsAmount { get; set; }
        public int BonusCoins { get; set; }
        public int TotalCoins { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; } 
        public int? DiscountPercentage { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsOnSale { get; set; }
    }
}