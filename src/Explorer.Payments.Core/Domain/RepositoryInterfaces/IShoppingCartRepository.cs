using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart? GetByTouristId(int touristId);
        ShoppingCart Create(ShoppingCart cart);
        ShoppingCart Update(ShoppingCart cart);

    }
}
