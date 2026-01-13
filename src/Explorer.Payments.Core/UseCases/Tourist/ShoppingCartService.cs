using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class ShoppingCartService : IShoppingCartService, ICartPricingService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IMapper _mapper;
        private static readonly Random _rng = new Random();
        private readonly ITourInfoService _tourInfoService;
        private readonly ITouristBundleService _bundleService;

        public ShoppingCartService(IShoppingCartRepository cartRepository, ITourInfoService tourInfoService, ITouristBundleService bundleService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourInfoService = tourInfoService;
            _bundleService = bundleService;
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



        public ShoppingCartDto AddToCartWithPrice(int touristId, int tourId, decimal finalPrice)
        {
            var tour = _tourInfoService.Get(tourId);
            if (tour == null) throw new NotFoundException($"Tour {tourId} not found.");

            if (tour.Status == TourLifecycleStatus.Archived)
                throw new EntityValidationException("Archived tours cannot be added to cart.");

            if (tour.Status != TourLifecycleStatus.Published)
                throw new EntityValidationException("Only published tours can be added to cart.");

            var cart = _cartRepository.GetByTouristId(touristId) ?? new ShoppingCart(touristId);

            var previousTotal = cart.TotalPrice;

            cart.AddItem(tour.TourId, tour.Name, finalPrice);

            if (cart.Id == 0) cart = _cartRepository.Create(cart);
            else if (cart.TotalPrice != previousTotal) cart = _cartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto AddBundleToCart(int touristId, int bundleId)
        {
            var bundle = _bundleService.GetById(bundleId);

            if (bundle.Status != Explorer.Tours.API.Dtos.BundleDtoStatus.Published)
                throw new EntityValidationException("Only published bundles can be added to cart.");

            var cart = _cartRepository.GetByTouristId(touristId) ?? new ShoppingCart(touristId);

            cart.AddBundleItem(bundleId, bundle.Name, bundle.Price);

            cart = cart.Id == 0 ? _cartRepository.Create(cart) : _cartRepository.Update(cart);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto RemoveBundleFromCart(int touristId, int bundleId)
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

            cart.RemoveBundleItem(bundleId);

            cart = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}
