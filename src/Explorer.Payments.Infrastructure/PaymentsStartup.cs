using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases;
using Explorer.Payments.Core.UseCases.Administration;
using Explorer.Payments.Core.UseCases.Internal;
using Explorer.Payments.Core.UseCases.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Payments.Infrastructure.Database.Repositories;
using Explorer.Payments.Infrastructure.Internal;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.UseCases.Author;
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
         services.AddScoped<ITourPurchaseTokenService, TourPurchaseTokenService>();
         services.AddScoped<IWalletService, WalletService>();
         services.AddScoped<IWalletAdministrationService, WalletAdministrationService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ICartPricingService, ShoppingCartService>();
        services.AddScoped<IBundlePurchaseService, BundlePurchaseService>();
        services.AddScoped<IAffiliateCodeService, AffiliateCodeService>();
        services.AddScoped<IGroupTravelService, GroupTravelService>();
        services.AddScoped<ICoinsBundleService, CoinsBundleService>();
        services.AddScoped<ICoinsBundleSaleService, CoinsBundleSaleService>();
        services.AddScoped<IGiftCardService, GiftCardService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        
        services.AddScoped<IShoppingCartRepository, ShoppingCartDbRepository>(); 
        services.AddScoped<ITourPurchaseTokenRepository, TourPurchaseTokenDbRepository>();
        services.AddScoped<IWalletRepository, WalletDbRepository>();
        services.AddScoped<IPaymentRecordRepository, PaymentRecordDbRepository>();
        services.AddScoped<ICouponRepository, CouponDbRepository>();
        services.AddScoped<IAffiliateCodeRepository, AffiliateCodeDbRepository>();
        services.AddScoped<IGroupTravelRequestRepository, GroupTravelRequestDbRepository>();
        
        RegisterAdapters(services);

        services.AddScoped<ICoinsBundleRepository, CoinsBundleDbRepository>();
        services.AddScoped<ICoinsBundleSaleRepository, CoinsBundleSaleDbRepository>();
        services.AddScoped<ICoinsBundlePurchaseRepository, CoinsBundlePurchaseDbRepository>();
        services.AddScoped<IGiftCardRepository, GiftCardDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("payments"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<PaymentsContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "payments")));
    }

    private static void RegisterAdapters(IServiceCollection services)
    {
        var userServiceType = Type.GetType("Explorer.Stakeholders.API.Public.IUserService, Explorer.Stakeholders.API");
        var userDiscoveryServiceType = Type.GetType("Explorer.Stakeholders.API.Public.IUserDiscoveryService, Explorer.Stakeholders.API");
        var userRepositoryType = Type.GetType("Explorer.Stakeholders.Core.Domain.RepositoryInterfaces.IUserRepository, Explorer.Stakeholders.Core");
        var notificationServiceType = Type.GetType("Explorer.Stakeholders.API.Public.INotificationService, Explorer.Stakeholders.API");
        
        if (userServiceType != null && userDiscoveryServiceType != null)
        {
            services.AddScoped(typeof(IUserInfoService), sp =>
            {
                var userService = sp.GetRequiredService(userServiceType);
                var userDiscoveryService = sp.GetRequiredService(userDiscoveryServiceType);
                object? personIdResolver = userRepositoryType != null ? sp.GetService(userRepositoryType) : null;
                return Activator.CreateInstance(typeof(UserInfoServiceAdapter), userService, userDiscoveryService, personIdResolver!);
            });
        }
        
        if (notificationServiceType != null)
        {
            services.AddScoped(typeof(INotificationServiceInternal), sp =>
            {
                var notificationService = sp.GetRequiredService(notificationServiceType);
                return Activator.CreateInstance(typeof(NotificationServiceAdapter), notificationService);
            });
        }
    }
}