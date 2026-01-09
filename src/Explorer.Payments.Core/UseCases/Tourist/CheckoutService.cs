using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IMapper _mapper;

        public CheckoutService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IWalletRepository walletRepository,
            IPaymentRecordRepository paymentRecordRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _paymentRecordRepository = paymentRecordRepository;
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

            decimal totalPrice = 0;
            var itemsToPurchase = new List<OrderItem>();
            
            foreach (var item in cart.Items)
            {
                if (_tokenRepository.Exists(touristId, item.TourId)) continue;
                totalPrice += item.Price;
                itemsToPurchase.Add(item);
            }

            if (itemsToPurchase.Count == 0)
            {
                cart.Clear();
                _cartRepository.Update(cart);
                return new List<TourPurchaseTokenDto>();
            }

            if (wallet.Balance < totalPrice)
            {
                throw new System.InvalidOperationException($"Insufficient balance. Required: {totalPrice} AC, Available: {wallet.Balance} AC");
            }

            wallet.DeductBalance(totalPrice);
            _walletRepository.Update(wallet);

            var createdTokens = new List<TourPurchaseToken>();
            
            foreach (var item in itemsToPurchase)
            {
                var paymentRecord = new PaymentRecord(touristId, item.TourId, item.Price);
                _paymentRecordRepository.Create(paymentRecord);

                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _tokenRepository.Create(token);
                createdTokens.Add(saved);
            }

            cart.Clear();
            _cartRepository.Update(cart);

            return createdTokens.Select(_mapper.Map<TourPurchaseTokenDto>).ToList();
        }
    }
}
