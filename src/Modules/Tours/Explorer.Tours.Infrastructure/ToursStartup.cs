using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.Author;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.UseCases.Tourist;
using Npgsql;

namespace Explorer.Tours.Infrastructure;

public static class ToursStartup
{
    public static IServiceCollection ConfigureToursModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ToursProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<ITourService, TourService>();

        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();
      
        services.AddScoped<ITourProblemService, TourProblemService>();
        services.AddScoped<IHistoricalMonumentService, HistoricalMonumentService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<ITourRepository, TourDbRepository>();
        services.AddScoped<ITourProblemRepository, TourProblemRepository>();

        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentDbRepository>();
        services.AddScoped<IHistoricalMonumentRepository, HistoricalMonumentRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));
    }
}