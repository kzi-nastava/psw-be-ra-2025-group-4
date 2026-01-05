using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class MysteryTourService : IMysteryTourOfferService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMysteryTourOfferRepository _offerRepository;
        private readonly IShoppingCartService _cartService;
        private readonly IMapper _mapper;

        private static readonly Random _rng = new();

        public MysteryTourService(
            ITourRepository tourRepository,
            IMysteryTourOfferRepository offerRepository,
            IShoppingCartService cartService,
            IMapper mapper)
        {
            _tourRepository = tourRepository;
            _offerRepository = offerRepository;
            _cartService = cartService;
            _mapper = mapper;
        }

        public MysteryTourOfferDto GetOrCreate(int touristId)
        {
            var existing = _offerRepository.GetActiveForTourist(touristId);
            if (existing != null)
                return Enrich(existing);

            var tours = _tourRepository.GetPublished()?.ToList() ?? new();
            if (!tours.Any())
                throw new NotFoundException("No tours available.");

            var tour = tours[_rng.Next(tours.Count)];
            var discount = _rng.Next(10, 41); 

            var offer = new MysteryTourOffer(touristId, (int)tour.Id, discount);
            var created = _offerRepository.Create(offer);

            return Enrich(created);
        }

        public ShoppingCartDto Redeem(Guid offerId, int touristId)
        {
            var offer = _offerRepository.GetActiveForTourist(touristId)
                ?? throw new NotFoundException("Offer not found.");

            if (offer.Id != offerId)
                throw new InvalidOperationException("Invalid offer.");

            try
            {
                offer.Redeem(); 
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var tour = _tourRepository.GetById(offer.TourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

            var finalPrice = Math.Round(tour.Price * (100 - offer.DiscountPercent) / 100m, 2);

            var cart = _cartService.AddToCartWithPrice(touristId, (int)tour.Id, finalPrice);

            _offerRepository.Update(offer);
            return cart;
        }

        private MysteryTourOfferDto Enrich(MysteryTourOffer offer)
        {
            var dto = _mapper.Map<MysteryTourOfferDto>(offer);

            var tour = _tourRepository.GetById(offer.TourId);
            if (tour != null)
            {
                dto.TourName = tour.Name;
                dto.OriginalPrice = tour.Price;
                dto.DiscountedPrice = Math.Round(tour.Price * (100 - offer.DiscountPercent) / 100m, 2);
            }

            return dto;
        }
    }

}
