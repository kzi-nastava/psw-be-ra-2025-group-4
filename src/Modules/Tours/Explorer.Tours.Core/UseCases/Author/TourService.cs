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
            var all = _tourRepository.GetByAuthor(authorId).ToList();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public TourDto GetByIdForAuthor(int authorId, int id)
        {
            var tour = _tourRepository.GetById(id);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            return _mapper.Map<TourDto>(tour);
        }

        public TourDto Create(CreateUpdateTourDto dto, int authorId)
        {
            var tour = new Tour(dto.Name, dto.Description, (TourDifficulty)dto.Difficulty, authorId, dto.Tags);
            tour.SetStatus(TourStatus.Draft);
            tour.SetPrice(0);

            var created = _tourRepository.Create(tour);
            return _mapper.Map<TourDto>(created);
        }

        public TourDto Update(int id, CreateUpdateTourDto dto, int authorId)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            tour.Update(dto.Name, dto.Description, (TourDifficulty)dto.Difficulty, dto.Tags);

            var updated = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(updated);
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
    }
}
