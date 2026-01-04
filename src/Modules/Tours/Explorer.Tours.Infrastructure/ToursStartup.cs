using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Mappers;
using Explorer.Tours.Core.UseCases.Administration;
using Explorer.Tours.Core.UseCases.Author;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.UseCases.Internal;
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
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<ITourService, TourService>();

        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuizSubmissionService, QuizSubmissionService>();

        services.AddScoped<ITourPreferencesService, TourPreferencesService>();
        services.AddScoped<ITouristEquipmentService, TouristEquipmentService>();

        services.AddScoped<ITourProblemService, TourProblemService>();

        // Both features preserved
        services.AddScoped<IHistoricalMonumentService, HistoricalMonumentService>();
        services.AddScoped<ITourPointService, TourPointService>();

        

        services.AddScoped<ITourExecutionService, TourExecutionService>();
        services.AddScoped<ITourSearchService, TourSearchService>();
        services.AddScoped<ITourPointSecretService, TourPointSecretService>();

        services.AddScoped<ITourReviewService, TourReviewService>();
        services.AddScoped<ITourInfoService, TourInfoService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IEquipmentRepository, EquipmentDbRepository>();
        services.AddScoped<IFacilityRepository, FacilityDbRepository>();

        services.AddScoped<ITourRepository, TourDbRepository>();

        services.AddScoped<IQuizRepository, QuizDbRepository>();
        services.AddScoped<IQuizAnswerRepository, QuizAnswerDbRepository>();

        services.AddScoped<ITourPreferencesRepository, TourPreferencesDbRepository>();

        services.AddScoped<ITourProblemRepository, TourProblemRepository>();
        services.AddScoped<ITouristEquipmentRepository, TouristEquipmentDbRepository>();

        // Both repositories preserved
        services.AddScoped<IHistoricalMonumentRepository, HistoricalMonumentRepository>();
        services.AddScoped<ITourPointRepository, TourPointDbRepository>();
        services.AddScoped<ITourExecutionRepository, TourExecutionDbRepository>();
        services.AddScoped<ITourReviewRepository, TourReviewDbRepository>();



        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("tours"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ToursContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "tours")));

        

        

    }
}
