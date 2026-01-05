using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IWalletService
    {
        WalletDto GetWallet(int touristId);
    }
}

