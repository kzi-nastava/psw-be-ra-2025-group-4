using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Tours.Tests;

using Explorer.Encounters.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;

public class ToursTestFactory : BaseTestFactory<ToursContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ToursContext>));
        services.Remove(descriptor!);
        services.AddDbContext<ToursContext>(SetupTestContext());

        var paymentsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        if (paymentsDescriptor != null) services.Remove(paymentsDescriptor);
        services.AddDbContext<PaymentsContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EncountersContext>));
        if (descriptor != null) services.Remove(descriptor);
        services.AddDbContext<EncountersContext>(SetupTestContext());

        return services;
    }
}
