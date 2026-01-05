using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetForTourist(int touristId);
        ShoppingCartDto AddToCart(int touristId, int tourId);

        ShoppingCartDto RemoveFromCart(int touristId, int tourId);
        ShoppingCartDto ClearCart(int touristId);

    }
}
