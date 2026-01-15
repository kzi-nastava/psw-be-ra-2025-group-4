using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain.Repositories;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Core.Mappers;
using Explorer.Encounters.Core.UseCases;
using Explorer.Encounters.Infrastructure.Database;
using Explorer.Encounters.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Encounters.Infrastructure
{
    public static class EncountersStartup
    {
        public static IServiceCollection ConfigureEncountersModule(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(EncountersProfile).Assembly);
            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            //TODO
            services.AddScoped<IEncounterService, EncounterService>();
            services.AddScoped<ITouristEncounterService, TouristEncounterService>();
            
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            //TODO
            services.AddScoped<IEncounterRepository, EncounterDbRepository>();
            services.AddScoped<IEncounterExecutionRepository, EncounterExecutionDbRepository>();
            services.AddScoped<IHiddenLocationEncounterRepository, HiddenLocationEncounterDbRepository>();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("encounters"));
            dataSourceBuilder.EnableDynamicJson();

            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<EncountersContext>(opt =>
            opt.UseNpgsql(dataSource, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "encounters")));
        }
    }
}