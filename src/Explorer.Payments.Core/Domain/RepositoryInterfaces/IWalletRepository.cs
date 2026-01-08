namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IWalletRepository
    {
        Wallet? GetByTouristId(int touristId);
        Wallet Create(Wallet wallet);
        Wallet Update(Wallet wallet);
        bool Exists(int touristId);
    }
}

