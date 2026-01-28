using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
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
        private readonly IBundleInfoService _bundleService;
        private readonly IUserInfoService _userInfoService;

        public ShoppingCartService(IShoppingCartRepository cartRepository, ITourInfoService tourInfoService, IBundleInfoService bundleService, IUserInfoService userInfoService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourInfoService = tourInfoService;
            _bundleService = bundleService;
            _userInfoService = userInfoService;
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

            bool needsUpdate = false;

            foreach (var item in cart.Items.ToList())
            {
                try
                {
                    var tourInfo = _tourInfoService.Get(item.TourId);

                    if (tourInfo.Price != item.Price)
                    {
                        cart.UpdateItemPrice(item.TourId, tourInfo.Price);
                        needsUpdate = true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            if (needsUpdate)
            {
                cart = _cartRepository.Update(cart);
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
            var bundle = _bundleService.Get(bundleId);

            if (bundle.Status != BundleLifecycleStatus.Published)
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

        public ShoppingCartDto SetGiftRecipient(int touristId, int tourId, string? recipientUsername)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                throw new NotFoundException("Shopping cart not found.");
            }

            if (string.IsNullOrWhiteSpace(recipientUsername))
            {
                cart.SetRecipientForItem(tourId, null);
                cart = _cartRepository.Update(cart);
                return _mapper.Map<ShoppingCartDto>(cart);
            }

            var recipientUser = _userInfoService.GetUserByUsername(recipientUsername);
            if (recipientUser == null)
            {
                throw new EntityValidationException("Recipient user not found.");
            }

            if (recipientUser.Id == touristId)
            {
                throw new EntityValidationException("Cannot gift a tour to yourself.");
            }

            cart.SetRecipientForItem(tourId, (int)recipientUser.Id);
            cart = _cartRepository.Update(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}
