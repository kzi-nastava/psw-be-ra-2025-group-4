using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourService(ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public PagedResult<TourDto> GetPagedByAuthor(int authorId, int page, int pageSize)
        {
            var all = _tourRepository.GetByAuthor(authorId)
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public TourDto GetByIdForAuthor(int authorId, int id)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            // Mapiramo u DTO
            var dto = _mapper.Map<TourDto>(tour);

            // Ako DTO ima Points, sortiramo ih po Order
            if (dto.Points != null)
            {
                dto.Points = dto.Points
                    .OrderBy(p => p.Order)
                    .ToList();
            }

            return dto;
        }

        public TourDto GetById(int id)
        {
            var tour = _tourRepository.GetById(id);

            var dto = _mapper.Map<TourDto>(tour);

            if (dto.Points != null)
            {
                dto.Points = dto.Points
                    .OrderBy(p => p.Order)
                    .ToList();
            }

            return dto;
        }

        public TourDto Create(CreateUpdateTourDto dto, int authorId)
        {
            var durations = dto.TransportDuration.Select(_mapper.Map<TourTransportDuration>).ToList();
            var tour = new Tour(dto.Name, dto.Description,
                (TourDifficulty)dto.Difficulty, authorId, durations, dto.Tags);

            var created = _tourRepository.Create(tour);

            // Vraćamo sveže učitanu turu (sa Include Points u repozitorijumu)
            return GetByIdForAuthor(authorId, (int)created.Id);
        }

        public TourDto Update(int id, CreateUpdateTourDto dto, int authorId)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            var durations = dto.TransportDuration.Select(_mapper.Map<TourTransportDuration>).ToList();

            tour.Update(dto.Name, dto.Description,
                (TourDifficulty)dto.Difficulty, durations, dto.Tags);

            _tourRepository.Update(tour);

            // Ponovo učitamo i vratimo pun DTO
            return GetByIdForAuthor(authorId, id);
        }

        public void DeleteForAuthor(int authorId, int id)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            if (tour.Status != TourStatus.Draft)
                throw new ForbiddenException("Only draft tours can be deleted.");

            _tourRepository.Delete(id);
        }

        public void Publish(int tourId, int authorId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.Publish();
        }

        public void Archive(int tourId, int authorId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.Archive();
        }

        public void SetPrice(int tourId, int authorId, decimal price)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.SetPrice(price);
        }

        public void AddEquipment(int tourId, int authorId, List<EquipmentDto> equipment)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            var equipmentMap = equipment.Select(_mapper.Map<Equipment>).ToList();
            tour.AddEquipments(equipmentMap);
        }

        public void AddTourPoint(int tourId, int authorId, TourPointDto tourPoint)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.AddTourPoint(_mapper.Map<TourPoint>(tourPoint));
        }
        
        public PagedResult<TourDto> GetPublishedAndArchived(int page, int pageSize)
        {
            var all = _tourRepository.GetPublishedAndArchived()
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(t =>
            {
                var dto = _mapper.Map<TourDto>(t);
                if (dto.Points != null)
                {
                    dto.Points = dto.Points.OrderBy(p => p.Order).ToList();
                }
                return dto;
            }).ToList();

            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public PagedResult<TourDto> GetPublished(int page, int pageSize)
        {
            var all = _tourRepository.GetPublished()
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(t =>
            {
                var dto = _mapper.Map<TourDto>(t);
                if (dto.Points != null)
                {
                    dto.Points = dto.Points.OrderBy(p => p.Order).ToList();
                }
                return dto;
            }).ToList();

            return new PagedResult<TourDto>(mapped, all.Count);
        }
    }
}
