using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public ShoppingCartDto GetForTourist(int touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);

            if (cart == null)
            {
                return new ShoppingCartDto
                {
                    TouristId = touristId,
                    Items = new(),
                    TotalPrice = 0
                };
            }

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto AddToCart(int touristId, AddToCartRequestDto request)
        {
            if (request.Status == "Archived")
                throw new EntityValidationException("Archived tours cannot be added to cart.");

            if (request.Status != "Published")
                throw new EntityValidationException("Only published tours can be added to cart.");

            var cart = _cartRepository.GetByTouristId(touristId) ?? new ShoppingCart(touristId);
            var previousTotal = cart.TotalPrice;

            cart.AddItem(request.TourId, request.TourName, request.Price);

            if (cart.Id == 0) cart = _cartRepository.Create(cart);
            else if (cart.TotalPrice != previousTotal) cart = _cartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto RemoveFromCart(int touristId, int tourId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);

            if (cart == null)
            {
                return new ShoppingCartDto
                {
                    TouristId = touristId,
                    Items = new(),
                    TotalPrice = 0
                };
            }

            var previousTotal = cart.TotalPrice;
            cart.RemoveItem(tourId);

            if (cart.TotalPrice == previousTotal)
                return _mapper.Map<ShoppingCartDto>(cart);

            cart = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}
