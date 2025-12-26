using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Tours.API.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Explorer.Payments.Tests;

public class PaymentsTestFactory : BaseTestFactory<PaymentsContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        services.Remove(descriptor!);

        services.AddDbContext<PaymentsContext>(SetupTestContext());
        services.RemoveAll<ITourInfoService>();
        services.AddSingleton<ITourInfoService, FakeTourInfoService>();

        return services;
    }

    private class FakeTourInfoService : ITourInfoService
    {
        public TourInfoDto Get(int tourId)
        {
            
            return new TourInfoDto
            {
                TourId = tourId,
                Name = $"Tour {tourId}",
                Price = 20m,
                Status = TourLifecycleStatus.Published
            };
        }
    }
}
