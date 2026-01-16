using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Internal
{
    public class BundleInfoService : IBundleInfoService
    {
        private readonly IBundleRepository _bundleRepository;

        public BundleInfoService(IBundleRepository bundleRepository)
        {
            _bundleRepository = bundleRepository;
        }

        public BundleInfoDto Get(int bundleId)
        {
            var bundle = _bundleRepository.GetById(bundleId);
            if (bundle == null)
                throw new NotFoundException($"Bundle {bundleId} not found.");

            return new BundleInfoDto
            {
                Id = (int)bundle.Id,
                Name = bundle.Name,
                Price = bundle.Price,
                Status = MapStatus(bundle.Status),
                Tours = bundle.Tours.Select(t => new BundleTourInfoDto { Id = (int)t.Id }).ToList()
            };
        }

        private static BundleLifecycleStatus MapStatus(BundleStatus status)
        {
            return status switch
            {
                BundleStatus.Published => BundleLifecycleStatus.Published,
                BundleStatus.Archived => BundleLifecycleStatus.Archived,
                _ => BundleLifecycleStatus.Draft
            };
        }
    }
}

