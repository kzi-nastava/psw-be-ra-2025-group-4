using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;
using System;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class BundleService : IBundleService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public BundleService(IBundleRepository bundleRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public PagedResult<BundleDto> GetPagedByAuthor(int authorId, int page, int pageSize)
        {
            var all = _bundleRepository.GetByAuthor(authorId)
                                     .OrderBy(b => b.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(b => MapToDto(b)).ToList();
            return new PagedResult<BundleDto>(mapped, all.Count);
        }

        public BundleDto GetByIdForAuthor(int authorId, int id)
        {
            var bundle = _bundleRepository.GetById(id);

            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("Not your bundle.");

            return MapToDto(bundle);
        }

        public BundleDto Create(CreateBundleDto dto, int authorId)
        {
            if (dto.TourIds == null || dto.TourIds.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");

            if (dto.TourIds.Count < 2)
                throw new ArgumentException("Bundle must contain at least 2 published tours.");

            var tours = new List<Tour>();
            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
                if (tour.Status != TourStatus.Published)
                    throw new ArgumentException($"Tour {tourId} must be published to be added to a bundle.");
                tours.Add(tour);
            }

            var bundle = new Bundle(dto.Name, dto.Price, authorId, tours);
            var created = _bundleRepository.Create(bundle);
            return MapToDto(created);
        }

        public BundleDto Update(int id, UpdateBundleDto dto, int authorId)
        {
            var bundle = _bundleRepository.GetById(id);

            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("Not your bundle.");

            if (dto.TourIds == null || dto.TourIds.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");

            var tours = new List<Tour>();
            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourRepository.GetById(tourId);
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException($"Tour {tourId} does not belong to you.");
                if (tour.Status != TourStatus.Published)
                    throw new ArgumentException($"Tour {tourId} must be published to be added to a bundle.");
                tours.Add(tour);
            }

            if (bundle.Status == BundleStatus.Published && tours.Count < 2)
            {
                throw new ArgumentException("Published bundle must contain at least 2 published tours.");
            }

            bundle.Update(dto.Name, dto.Price, tours);
            var updated = _bundleRepository.Update(bundle);
            return MapToDto(updated);
        }

        public void Delete(int id, int authorId)
        {
            var bundle = _bundleRepository.GetById(id);

            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("Not your bundle.");

            bundle.Delete();
            _bundleRepository.Delete(id);
        }

        public void Publish(int id, int authorId)
        {
            var bundle = _bundleRepository.GetById(id);

            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("Not your bundle.");

            bundle.Publish();
            _bundleRepository.Update(bundle);
        }

        public void Archive(int id, int authorId)
        {
            var bundle = _bundleRepository.GetById(id);

            if (bundle.AuthorId != authorId)
                throw new ForbiddenException("Not your bundle.");

            bundle.Archive();
            _bundleRepository.Update(bundle);
        }

        private BundleDto MapToDto(Bundle bundle)
        {
            var dto = new BundleDto
            {
                Id = (int)bundle.Id,
                Name = bundle.Name,
                Price = bundle.Price,
                AuthorId = bundle.AuthorId,
                Status = (BundleDtoStatus)bundle.Status,
                Tours = bundle.Tours.Select(t => _mapper.Map<TourDto>(t)).ToList(),
                TotalToursPrice = bundle.Tours.Sum(t => t.Price)
            };
            return dto;
        }
    }
}

