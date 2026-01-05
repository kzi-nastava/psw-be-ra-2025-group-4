using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ITourInfoService _tourInfoService;

        public ShoppingCartService(IShoppingCartRepository cartRepository, ITourInfoService tourInfoService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourInfoService = tourInfoService;
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

        public ShoppingCartDto AddToCart(int touristId, int tourId)
        {
            var tour = _tourInfoService.Get(tourId);

            if (tour.Status == TourLifecycleStatus.Archived)
                throw new EntityValidationException("Archived tours cannot be added to cart.");

            if (tour.Status != TourLifecycleStatus.Published)
                throw new EntityValidationException("Only published tours can be added to cart.");

            var cart = _cartRepository.GetByTouristId(touristId) ?? new ShoppingCart(touristId);

            cart.AddItem(tour.TourId, tour.Name, tour.Price);

            cart = cart.Id == 0 ? _cartRepository.Create(cart) : _cartRepository.Update(cart);

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

            cart.RemoveItem(tourId);

            cart = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto ClearCart(int touristId)
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

            cart.Clear();

            cart = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }



    }
}
