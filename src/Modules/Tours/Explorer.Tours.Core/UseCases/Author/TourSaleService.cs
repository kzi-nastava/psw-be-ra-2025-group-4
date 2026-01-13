using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class TourSaleService : ITourSaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourSaleService(ISaleRepository saleRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public List<SaleDto> GetByAuthor(int authorId)
        {
            var sales = _saleRepository.GetByAuthor(authorId);
            return sales.Select(s => MapToDto(s)).ToList();
        }

        public SaleDto GetById(int id, int authorId)
        {
            var sale = _saleRepository.GetById(id);

            if (sale.AuthorId != authorId)
                throw new ForbiddenException("Not your sale.");

            return MapToDto(sale);
        }

        public SaleDto Create(SaleCreateDto dto, int authorId)
        {
            // Validacija da su sve ture autora
            if (dto.TourIds == null || dto.TourIds.Count == 0)
                throw new ArgumentException("Sale must contain at least one tour.");

            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
            }

            var sale = new Sale(authorId, dto.TourIds, dto.StartDate, dto.EndDate, dto.DiscountPercent);
            var created = _saleRepository.Create(sale);
            return MapToDto(created);
        }

        public SaleDto Update(int id, SaleUpdateDto dto, int authorId)
        {
            var sale = _saleRepository.GetById(id);

            if (sale.AuthorId != authorId)
                throw new ForbiddenException("Not your sale.");

            // Validacija da su sve ture autora
            if (dto.TourIds == null || dto.TourIds.Count == 0)
                throw new ArgumentException("Sale must contain at least one tour.");

            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
            }

            sale.Update(dto.TourIds, dto.StartDate, dto.EndDate, dto.DiscountPercent);
            var updated = _saleRepository.Update(sale);
            return MapToDto(updated);
        }

        public void Delete(int id, int authorId)
        {
            var sale = _saleRepository.GetById(id);

            if (sale.AuthorId != authorId)
                throw new ForbiddenException("Not your sale.");

            _saleRepository.Delete(id);
        }

        private SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = (int)sale.Id,
                AuthorId = sale.AuthorId,
                TourIds = sale.TourIds,
                StartDate = sale.StartDate,
                EndDate = sale.EndDate,
                DiscountPercent = sale.DiscountPercent,
                IsActive = sale.IsActive,
                IsCurrentlyActive = sale.IsCurrentlyActive()
            };
        }
    }
}

