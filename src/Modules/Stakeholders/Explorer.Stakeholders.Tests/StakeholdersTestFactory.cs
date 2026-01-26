using Explorer.BuildingBlocks.Tests;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Stakeholders.Tests;

public class StakeholdersTestFactory : BaseTestFactory<StakeholdersContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StakeholdersContext>));
        services.Remove(descriptor!);
        services.AddDbContext<StakeholdersContext>(SetupTestContext());

        var paymentsDescriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<PaymentsContext>));
        if (paymentsDescriptor != null) services.Remove(paymentsDescriptor);

        services.AddDbContext<PaymentsContext>(SetupTestContext());

        return services;
    }
}