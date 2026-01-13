using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Administration
{
    public class WalletAdministrationService : IWalletAdministrationService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletAdministrationService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public void AddBalance(int touristId, decimal amount)
        {
            if (amount < 0)
                throw new System.ArgumentException("Amount cannot be negative.", nameof(amount));

            var wallet = _walletRepository.GetByTouristId(touristId);
            
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }

            if (amount > 0)
            {
                wallet.AddBalance(amount);
                _walletRepository.Update(wallet);
            }
        }
    }
}

