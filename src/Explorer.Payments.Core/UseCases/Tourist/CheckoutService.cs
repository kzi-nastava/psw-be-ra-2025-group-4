using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly ITourInfoService _tourInfoService;
        private readonly IBundlePurchaseService _bundlePurchaseService;
        private readonly IMapper _mapper;

        public CheckoutService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IWalletRepository walletRepository,
            IPaymentRecordRepository paymentRecordRepository,
            ITourInfoService tourInfoService,
            IBundlePurchaseService bundlePurchaseService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _paymentRecordRepository = paymentRecordRepository;
            _tourInfoService = tourInfoService;
            _bundlePurchaseService = bundlePurchaseService;
            _mapper = mapper;
        }

        public List<TourPurchaseTokenDto> Checkout(int touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                throw new System.InvalidOperationException("Shopping cart is empty.");
            }

            var wallet = _walletRepository.GetByTouristId(touristId);
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }

            decimal tourTotalPrice = 0;
            var tourItemsToPurchase = new List<(OrderItem Item, decimal CurrentPrice)>();
            var bundleItemsToPurchase = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                if (item.BundleId.HasValue)
                {
                    if (_paymentRecordRepository.ExistsForBundle(touristId, item.BundleId.Value)) continue;
                    bundleItemsToPurchase.Add(item);
                }
                else
                {
                    if (_tokenRepository.Exists(touristId, item.TourId)) continue;

                    var tourInfo = _tourInfoService.Get(item.TourId);
                    decimal currentPrice = tourInfo.Price;

                    tourTotalPrice += currentPrice;
                    tourItemsToPurchase.Add((item, currentPrice));
                }
            }

            if (tourItemsToPurchase.Count == 0 && bundleItemsToPurchase.Count == 0)
            {
                cart.Clear();
                _cartRepository.Update(cart);
                return new List<TourPurchaseTokenDto>();
            }

            decimal bundleTotalPrice = bundleItemsToPurchase.Sum(item => item.Price);
            decimal totalPrice = tourTotalPrice + bundleTotalPrice;

            if (wallet.Balance < totalPrice)
            {
                throw new System.InvalidOperationException($"Insufficient balance. Required: {totalPrice} AC, Available: {wallet.Balance} AC");
            }

            if (tourTotalPrice > 0)
            {
                wallet.DeductBalance(tourTotalPrice);
                _walletRepository.Update(wallet);
            }

            var createdTokens = new List<TourPurchaseTokenDto>();

            foreach (var (item, currentPrice) in tourItemsToPurchase)
            {
                var paymentRecord = new PaymentRecord(touristId, item.TourId, currentPrice);
                _paymentRecordRepository.Create(paymentRecord);

                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _tokenRepository.Create(token);
                createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            foreach (var item in bundleItemsToPurchase)
            {
                var bundleTokens = _bundlePurchaseService.PurchaseBundle(touristId, item.BundleId.Value);
                createdTokens.AddRange(bundleTokens);
            }

            cart.Clear();
            _cartRepository.Update(cart);

            return createdTokens;
        }

        public List<TourPurchaseTokenDto> GetPurchaseTokens(int touristId)
        {
            var tokens = _tokenRepository.GetByTouristId(touristId);
            return tokens.Select(t => _mapper.Map<TourPurchaseTokenDto>(t)).ToList();
        }
    }
}