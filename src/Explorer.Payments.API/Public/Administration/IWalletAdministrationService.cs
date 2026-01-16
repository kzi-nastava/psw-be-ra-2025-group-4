namespace Explorer.Payments.API.Public.Administration
{
    public interface IWalletAdministrationService
    {
        void AddBalance(int touristId, decimal amount);
    }
}

