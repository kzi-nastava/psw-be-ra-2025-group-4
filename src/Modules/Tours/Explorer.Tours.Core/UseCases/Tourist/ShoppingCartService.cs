using System;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
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
            
            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
            {
                throw new InvalidOperationException($"Tour {tourId} not found.");
            }

            
            if (tour.Status == TourStatus.Archived)
            {
                throw new InvalidOperationException("Archived tours cannot be added to cart.");
            }

            if (tour.Status != TourStatus.Published)
            {
                throw new InvalidOperationException("Only published tours can be added to cart.");
            }

            
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
            }

            var previousTotal = cart.TotalPrice;

            
            cart.AddItem(tour.Id, tour.Name, tour.Price);

            
            if (cart.Id == 0)
            {
                
                cart = _cartRepository.Create(cart);
            }
            else if (cart.TotalPrice != previousTotal)
            {
                
                cart = _cartRepository.Update(cart);
            }

            return _mapper.Map<ShoppingCartDto>(cart);
        }
    }
}