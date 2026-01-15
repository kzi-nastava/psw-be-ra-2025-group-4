using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TouristBundleService : ITouristBundleService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly IMapper _mapper;

        public TouristBundleService(IBundleRepository bundleRepository, IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _mapper = mapper;
        }

        public PagedResult<BundleDto> GetPublished(int page, int pageSize)
        {
            var all = _bundleRepository.GetAll()
                .Where(b => b.Status == BundleStatus.Published)
                .OrderBy(b => b.Id)
                .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(b => MapToDto(b)).ToList();
            return new PagedResult<BundleDto>(mapped, all.Count);
        }

        public BundleDto GetById(int id)
        {
            var bundle = _bundleRepository.GetById(id);
            
            if (bundle.Status != BundleStatus.Published)
                throw new System.ArgumentException("Bundle is not published.");

            return MapToDto(bundle);
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

