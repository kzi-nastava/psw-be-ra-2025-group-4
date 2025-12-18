using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart? GetByTouristId(int touristId);
        ShoppingCart Create(ShoppingCart cart);
        ShoppingCart Update(ShoppingCart cart);
    }
}
