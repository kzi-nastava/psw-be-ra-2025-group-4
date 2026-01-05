using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetForTourist(int touristId);
        ShoppingCartDto AddToCart(int touristId, int tourId);
        ShoppingCartDto RemoveFromCart(int touristId, int tourId);
        public ShoppingCartDto AddToCartWithPrice(int touristId, int tourId, decimal finalPrice);
    }
}
