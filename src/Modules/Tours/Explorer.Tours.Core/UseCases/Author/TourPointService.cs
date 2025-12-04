using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class TourPointService : ITourPointService
    {
        private readonly ITourPointRepository _pointRepo;
        private readonly ITourRepository _tourRepo;
        private readonly IMapper _mapper;

        public TourPointService(ITourPointRepository pointRepo, ITourRepository tourRepo, IMapper mapper)
        {
            _pointRepo = pointRepo;
            _tourRepo = tourRepo;
            _mapper = mapper;
        }

        public List<TourPointDto> GetByTour(int tourId, int authorId)
        {
            var tour = _tourRepo.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");

            var points = _pointRepo.GetByTour(tourId);
            return points.Select(_mapper.Map<TourPointDto>).ToList();
        }

        public TourPointDto Create(int tourId, TourPointDto dto, int authorId)
        {
            var tour = _tourRepo.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");

            var current = _pointRepo.GetByTour(tourId);
            var order = current.Any() ? current.Max(p => p.Order) + 1 : 1;
            var point = new TourPoint(tourId, dto.Name, dto.Description, dto.Latitude, dto.Longitude, order);

            var created = _pointRepo.Create(point);
            return _mapper.Map<TourPointDto>(created);
        }

        public TourPointDto Update(int pointId, TourPointDto dto, int authorId)
        {
            var point = _pointRepo.Get(pointId);
            var tour = _tourRepo.GetById((int)point.TourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");

            point.Update(dto.Name, dto.Description, dto.Latitude, dto.Longitude, dto.Order);
            var updated = _pointRepo.Update(point);

            return _mapper.Map<TourPointDto>(updated);
        }

        public void Delete(int pointId, int authorId)
        {
            var point = _pointRepo.Get(pointId);
            var tour = _tourRepo.GetById((int)point.TourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");

            _pointRepo.Delete(pointId);
        }
    }
}
