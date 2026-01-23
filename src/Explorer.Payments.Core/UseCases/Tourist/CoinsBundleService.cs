using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class CoinsBundleService : ICoinsBundleService
    {
        private readonly ICoinsBundleRepository _bundleRepository;
        private readonly ICoinsBundleSaleRepository _saleRepository;
        private readonly ICoinsBundlePurchaseRepository _purchaseRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public CoinsBundleService(
            ICoinsBundleRepository bundleRepository,
            ICoinsBundleSaleRepository saleRepository,
            ICoinsBundlePurchaseRepository purchaseRepository,
            IWalletRepository walletRepository,
            IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _saleRepository = saleRepository;
            _purchaseRepository = purchaseRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public List<CoinsBundleDto> GetAllBundles()
        {
            var bundles = _bundleRepository.GetAll();
            var bundleDtos = new List<CoinsBundleDto>();

            foreach (var bundle in bundles.OrderBy(b => b.DisplayOrder))
            {
                var dto = _mapper.Map<CoinsBundleDto>(bundle);
                dto.TotalCoins = bundle.GetTotalCoins();

                var activeSale = _saleRepository.GetActiveSaleForBundle((int)bundle.Id);
                if (activeSale != null && activeSale.IsCurrentlyActive())
                {
                    dto.IsOnSale = true;
                    dto.DiscountPercentage = (int)activeSale.DiscountPercentage;
                    dto.DiscountedPrice = activeSale.CalculateDiscountedPrice(bundle.Price);
                }
                else
                {
                    dto.IsOnSale = false;
                    dto.DiscountedPrice = null;
                    dto.DiscountPercentage = null;
                }

                bundleDtos.Add(dto);
            }

            return bundleDtos;
        }

        public CoinsBundleDto GetBundle(int id)
        {
            var bundle = _bundleRepository.Get(id);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle not found.");

            var dto = _mapper.Map<CoinsBundleDto>(bundle);
            dto.TotalCoins = bundle.GetTotalCoins();

            var activeSale = _saleRepository.GetActiveSaleForBundle((int)bundle.Id);
            if (activeSale != null && activeSale.IsCurrentlyActive())
            {
                dto.IsOnSale = true;
                dto.DiscountPercentage = (int)activeSale.DiscountPercentage;
                dto.DiscountedPrice = activeSale.CalculateDiscountedPrice(bundle.Price);
            }

            return dto;
        }

        public CoinsBundlePurchaseDto PurchaseBundle(int touristId, PurchaseCoinsBundleRequestDto request)
        {

            var bundle = _bundleRepository.Get(request.CoinsBundleId);
            if (bundle == null)
                throw new KeyNotFoundException("Bundle not found.");

            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var paymentMethod))
                throw new ArgumentException("Invalid payment method.");

            ValidatePaymentDetails(request, paymentMethod);

            var finalPrice = bundle.Price;
            var activeSale = _saleRepository.GetActiveSaleForBundle((int)bundle.Id);
            if (activeSale != null && activeSale.IsCurrentlyActive())
            {
                finalPrice = activeSale.CalculateDiscountedPrice(bundle.Price);
            }

            var transactionId = GenerateTransactionId(paymentMethod);

            var purchase = new CoinsBundlePurchase(
                touristId,
                (int)bundle.Id,
                bundle.Name,
                bundle.GetTotalCoins(),
                finalPrice,
                bundle.Price,
                paymentMethod,
                transactionId
            );
            _purchaseRepository.Create(purchase);

            var wallet = _walletRepository.GetByTouristId(touristId);
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }
            wallet.AddBalance(bundle.GetTotalCoins());
            _walletRepository.Update(wallet);

            return _mapper.Map<CoinsBundlePurchaseDto>(purchase);
        }

        public List<CoinsBundlePurchaseDto> GetPurchaseHistory(int touristId)
        {
            var purchases = _purchaseRepository.GetByTouristId(touristId);
            return _mapper.Map<List<CoinsBundlePurchaseDto>>(purchases);
        }

        private void ValidatePaymentDetails(PurchaseCoinsBundleRequestDto request, PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.CreditCard:
                    if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 13)
                        throw new ArgumentException("Invalid card number.");
                    if (string.IsNullOrWhiteSpace(request.CardHolderName))
                        throw new ArgumentException("Card holder name is required.");
                    if (string.IsNullOrWhiteSpace(request.ExpiryDate))
                        throw new ArgumentException("Expiry date is required.");
                    if (string.IsNullOrWhiteSpace(request.CVV) || request.CVV.Length < 3)
                        throw new ArgumentException("Invalid CVV.");
                    break;

                case PaymentMethod.PayPal:
                    if (string.IsNullOrWhiteSpace(request.PayPalEmail) || !request.PayPalEmail.Contains("@"))
                        throw new ArgumentException("Valid PayPal email is required.");
                    break;

                case PaymentMethod.GiftCard:
                    if (string.IsNullOrWhiteSpace(request.GiftCardCode))
                        throw new ArgumentException("Gift card code is required.");
                    if (request.GiftCardCode.Length < 10)
                        throw new ArgumentException("Invalid gift card code.");
                    break;
            }
        }

        private string GenerateTransactionId(PaymentMethod method)
        {
            var prefix = method switch
            {
                PaymentMethod.CreditCard => "CC",
                PaymentMethod.PayPal => "PP",
                PaymentMethod.GiftCard => "GC",
                _ => "TX"
            };
            return $"{prefix}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}