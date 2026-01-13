using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class BundlePurchaseService : IBundlePurchaseService
    {
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly ITouristBundleService _bundleService;
        private readonly IMapper _mapper;

        public BundlePurchaseService(
            ITourPurchaseTokenRepository tokenRepository,
            IWalletRepository walletRepository,
            IPaymentRecordRepository paymentRecordRepository,
            ITouristBundleService bundleService,
            IMapper mapper)
        {
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _paymentRecordRepository = paymentRecordRepository;
            _bundleService = bundleService;
            _mapper = mapper;
        }

        public List<TourPurchaseTokenDto> PurchaseBundle(int touristId, int bundleId)
        {
            if (_paymentRecordRepository.ExistsForBundle(touristId, bundleId))
            {
                throw new System.InvalidOperationException("Bundle has already been purchased.");
            }

            var bundle = _bundleService.GetById(bundleId);

            var wallet = _walletRepository.GetByTouristId(touristId);
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }

            if (wallet.Balance < bundle.Price)
            {
                throw new System.InvalidOperationException($"Insufficient balance. Required: {bundle.Price} AC, Available: {wallet.Balance} AC");
            }

            var toursToPurchase = bundle.Tours
                .Where(t => !_tokenRepository.Exists(touristId, t.Id))
                .ToList();

            if (toursToPurchase.Count == 0)
            {
                return new List<TourPurchaseTokenDto>();
            }

            wallet.DeductBalance(bundle.Price);
            _walletRepository.Update(wallet);

            var paymentRecord = new PaymentRecord(touristId, bundleId, bundle.Price, isBundle: true);
            _paymentRecordRepository.Create(paymentRecord);

            var createdTokens = new List<TourPurchaseToken>();
            foreach (var tour in toursToPurchase)
            {
                var token = new TourPurchaseToken(touristId, tour.Id);
                var saved = _tokenRepository.Create(token);
                createdTokens.Add(saved);
            }

            return createdTokens.Select(_mapper.Map<TourPurchaseTokenDto>).ToList();
        }
    }
}

