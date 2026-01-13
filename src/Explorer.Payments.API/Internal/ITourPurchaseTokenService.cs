namespace Explorer.Payments.API.Internal
{
    public interface ITourPurchaseTokenService
    {
        bool HasToken(int touristId, int tourId);
    }
}