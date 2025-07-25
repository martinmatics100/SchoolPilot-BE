

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Interfaces;
using SchoolPilot.Data.Postgres.Core;
using SchoolPilot.Data.Postgres.Core.Factories;
using SchoolPilot.Lookup.Data.Postgres.Core;

namespace SchoolPilot.Host.DIContainer.Modules
{
    public static class ConnectionModule
    {
        public static IServiceCollection AddDatabaseConnections(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Read Database Context
            services.AddScoped<ReadSchoolPilotContext, MyPostgresReadSchoolPilotContext>();

            // Register Read-Write Database Context
            services.AddScoped<ReadWriteSchoolPilotContext, MyPostgresReadWriteSchoolPilotContext>();

            // Register Read Database Context For Lookup
            services.AddScoped<ReadSchoolPilotLookupContext, MyPostgresReadSchoolPilotLookupContext>();

            // Register Read-Write Database Context For Lookup
            services.AddScoped<ReadWriteSchoolPilotLookupContext, MyPostgresReadWriteSchoolPilotLookupContext>();

            // Register Database Connection Factory
            services.AddScoped<IDatabaseConnectionFactory, PostgresDatabaseConnectionFactory>();

            return services;
        }
    }
}
