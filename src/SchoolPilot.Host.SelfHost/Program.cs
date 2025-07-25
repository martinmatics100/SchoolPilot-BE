using SchoolPilot.Data.Context;
using SchoolPilot.Host.SelfHost.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

ServiceConfiguration.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

PipelineConfiguration.ConfigurePipeline(app);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ReadWriteSchoolPilotContext>();
        await ServiceConfiguration.SeedRolesAsync(context);

        var lookupContext = services.GetRequiredService<ReadWriteSchoolPilotLookupContext>();
        await ServiceConfiguration.SeedCountriesAsync(lookupContext);
        await ServiceConfiguration.SeedStatesAsync(lookupContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles.");
        throw;
    }
}

app.Run();
