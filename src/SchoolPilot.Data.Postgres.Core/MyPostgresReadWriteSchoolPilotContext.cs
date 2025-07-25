using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Data.Postgres.Core
{
    public class MyPostgresReadWriteSchoolPilotContext : ReadWriteSchoolPilotContext
    {
        public string? ConnectionString { get; }
        private readonly IConfiguration _configuration;

        public MyPostgresReadWriteSchoolPilotContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MyPostgresReadWriteSchoolPilotContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConnectionString ?? _configuration.GetConnectionString("ReadWriteSchoolPilotDb");

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

    public class MyPostgresReadSchoolPilotContext : ReadSchoolPilotContext
    {
        public string? ConnectionString { get; }
        private readonly IConfiguration _configuration;

        public MyPostgresReadSchoolPilotContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MyPostgresReadSchoolPilotContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConnectionString ?? _configuration.GetConnectionString("ReadSchoolPilotDb");

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