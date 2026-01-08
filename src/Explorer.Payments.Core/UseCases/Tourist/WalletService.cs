using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public WalletDto GetWallet(int touristId)
        {
            var wallet = _walletRepository.GetByTouristId(touristId);
            
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }

            return _mapper.Map<WalletDto>(wallet);
        }
    }
}

