using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Mappers;
using Explorer.Encounters.Infrastructure.Database;
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
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            //TODO

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("encounters"));
            dataSourceBuilder.EnableDynamicJson();

            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<EncountersContext>(opt =>
            opt.UseNpgsql(dataSource, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "encounters")));
        }
    }
}