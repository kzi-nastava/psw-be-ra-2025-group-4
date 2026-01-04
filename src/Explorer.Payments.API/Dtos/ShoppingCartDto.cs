using System.Collections.Generic;

namespace Explorer.Payments.API.Dtos
{
    public class ShoppingCartDto
    {
        public int TouristId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
