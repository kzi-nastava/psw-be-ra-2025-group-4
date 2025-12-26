using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Payments.Infrastructure;

public static class PaymentsStartup
{
    public static IServiceCollection ConfigurePaymentsModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(PaymentsProfile).Assembly);

        SetupCore(services);
        SetupInfrastructure(services);
        services.AddAutoMapper(typeof(PaymentsProfile).Assembly);

        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        
         services.AddScoped<IShoppingCartService, ShoppingCartService>();
         services.AddScoped<ICheckoutService, CheckoutService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        
        services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>(); 
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<PaymentsContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
    }
}