using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }
    
    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();
        services.AddScoped<IDirectMessageService, DirectMessageService>();
        services.AddScoped<IClubService, ClubService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ITouristLocationService, TouristLocationService>();
        services.AddScoped<IClubMessageService, ClubMessageService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<INotificationService, NotificationService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<IDirectMessageRepository, DirectMessageDbRepository>();
        services.AddScoped<IClubRepository, ClubDbRepository>();
        services.AddTransient<IRatingRepository, RatingDbRepository>();
        services.AddScoped<ITouristLocationRepository, TouristLocationDbRepository>();

        services.AddScoped<IUserProfileRepository, UserProfileDbRepository>();
        services.AddScoped<IClubMessageRepository, ClubMessageDbRepository>();
        services.AddScoped<IFollowRepository, FollowDbRepository>();
        services.AddScoped<INotificationRepository, NotificationDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
