using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
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

        public PagedResult<TourDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(items, result.TotalCount);
        }

        public TourDto GetById(int id)
        {
            var tour = _tourRepository.GetById(id);
            return _mapper.Map<TourDto>(tour);
        }

        public TourDto Create(TourDto entity)
        {
            var tour = _mapper.Map<Tour>(entity);
            var result = _tourRepository.Create(tour);
            return _mapper.Map<TourDto>(result);
        }

        public TourDto Update(TourDto entity)
        {
            var tour = _mapper.Map<Tour>(entity);
            var result = _tourRepository.Update(tour);
            return _mapper.Map<TourDto>(result);
        }

        public void Delete(int id)
        {
            _tourRepository.Delete(id);
        }
        public IEnumerable<TourDto> GetByAuthor(int authorId)
        {
            var tours = _tourRepository.GetByAuthor(authorId);
            return tours.Select(_mapper.Map<TourDto>).ToList();
        }

    }
}
