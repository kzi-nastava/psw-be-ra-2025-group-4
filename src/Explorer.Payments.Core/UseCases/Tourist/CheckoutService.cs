using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IMapper _mapper;

        public CheckoutService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
        }

        public List<TourPurchaseTokenDto> Checkout(int touristId)
        {
            
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                throw new System.InvalidOperationException("Shopping cart is empty.");
            }

            var createdTokens = new List<TourPurchaseToken>();

            
            foreach (var item in cart.Items)
            {
                if (_tokenRepository.Exists(touristId, item.TourId)) continue;

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
