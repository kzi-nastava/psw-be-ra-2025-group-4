using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetForTourist(int touristId);
        ShoppingCartDto AddToCart(int touristId, int tourId);
        ShoppingCartDto AddBundleToCart(int touristId, int bundleId);
        ShoppingCartDto RemoveFromCart(int touristId, int tourId);
        ShoppingCartDto RemoveBundleFromCart(int touristId, int bundleId);
        ShoppingCartDto ClearCart(int touristId);
        ShoppingCartDto AddToCartWithPrice(int touristId, int tourId, decimal finalPrice);
    }
}
