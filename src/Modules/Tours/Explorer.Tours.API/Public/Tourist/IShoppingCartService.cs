using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetForTourist(int touristId);
        ShoppingCartDto AddToCart(int touristId, int tourId);
    }
}
