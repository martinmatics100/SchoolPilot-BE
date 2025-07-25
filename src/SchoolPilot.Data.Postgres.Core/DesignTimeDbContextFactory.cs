

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Data.Postgres.Core
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyPostgresReadWriteSchoolPilotContext>
    {
        public string? ReadSchoolPilotDbConnectionString { get; private set; } = string.Empty;
        public string? ReadWriteSchoolPilotDbConnectionString { get; private set; } = string.Empty;

        public DesignTimeDbContextFactory()
        {
            // Load configuration from appsettings.json
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Load connection strings
            ReadSchoolPilotDbConnectionString = configuration.GetConnectionString("ReadSchoolPilotDb");
            ReadWriteSchoolPilotDbConnectionString = configuration.GetConnectionString("ReadWriteSchoolPilotDb");

            if (string.IsNullOrEmpty(ReadSchoolPilotDbConnectionString) || string.IsNullOrEmpty(ReadWriteSchoolPilotDbConnectionString))
            {
                throw new InvalidOperationException("Connection strings are not configured in appsettings.json.");
            }
        }

        public MyPostgresReadWriteSchoolPilotContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SchoolPilotContext>();
            if (string.IsNullOrEmpty(ReadWriteSchoolPilotDbConnectionString))
            {
                throw new InvalidOperationException("ReadWriteSchoolPilotDbConnectionString cannot be null or empty.");
            }
            optionsBuilder.UseNpgsql(ReadWriteSchoolPilotDbConnectionString);

            return new MyPostgresReadWriteSchoolPilotContext(ReadWriteSchoolPilotDbConnectionString);
        }

    }
}
