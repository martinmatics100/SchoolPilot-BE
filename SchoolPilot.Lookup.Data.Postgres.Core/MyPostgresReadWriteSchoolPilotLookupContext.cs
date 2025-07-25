
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Lookup.Data.Postgres.Core
{
    public class MyPostgresReadWriteSchoolPilotLookupContext : ReadWriteSchoolPilotLookupContext
    {
        public string? ConnectionString { get; }
        private readonly IConfiguration _configuration;

        public MyPostgresReadWriteSchoolPilotLookupContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MyPostgresReadWriteSchoolPilotLookupContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConnectionString ?? _configuration.GetConnectionString("ReadWriteSchoolPilotLookupDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(180);
                    npgsqlOptions.SetPostgresVersion(new Version(12, 0));
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure all DateTime properties to properly handle UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        // Change to timestamp with time zone to properly store UTC
                        property.SetColumnType("timestamp with time zone");

                        // Add value converter to ensure UTC handling
                        property.SetValueConverter(
                            new ValueConverter<DateTime, DateTime>(
                                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));

                        // Set UTC defaults for audit fields
                        if (property.Name == "CreatedOn")
                        {
                            property.SetDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
                        }
                        else if (property.Name == "ModifiedOn")
                        {
                            property.SetDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
                        }
                    }
                }
            }
        }
    }

    public class MyPostgresReadSchoolPilotLookupContext : ReadSchoolPilotLookupContext
    {
        public string? ConnectionString { get; }
        private readonly IConfiguration _configuration;

        public MyPostgresReadSchoolPilotLookupContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MyPostgresReadSchoolPilotLookupContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConnectionString ?? _configuration.GetConnectionString("ReadSchoolPilotLookupDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            optionsBuilder
                .UseLazyLoadingProxies()
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // Add this line
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(180);
                    npgsqlOptions.SetPostgresVersion(new Version(12, 0));
                });
        }

    }
}
