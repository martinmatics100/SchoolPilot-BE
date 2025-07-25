using CommandLine;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Context;
using SchoolPilot.Host.DIContainer.Modules;

public class Program
{
    public static void Main(string[] args)
    {
        // Check if the application is running in a development environment
        bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var host = CreateHostBuilder(args).Build();
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        if (isDevelopment)
        {
            // Bypass command-line parsing and directly run migrations for development
            RunMigrations(serviceProvider, ContextType.SchoolPilot);
        }
        else
        {
            // Use command-line parsing for production or other environments
            var helpWriter = new StringWriter();
            var parser = new Parser(with => with.HelpWriter = helpWriter);
            parser.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!options.Enabled)
                    {
                        Console.WriteLine("Program did not receive -e or --enabled flag, to ensure you actually wanted to run the migrations.");
                        Environment.Exit(1);
                    }

                    RunMigrations(serviceProvider, options.DbContextType);
                })
                .WithNotParsed(errors =>
                {
                    if (errors.IsVersion() || errors.IsHelp())
                        Console.WriteLine(helpWriter.ToString());
                    else
                        Console.Error.WriteLine(helpWriter.ToString());

                    Environment.Exit(1);
                });
        }
    }

    private static void RunMigrations(IServiceProvider serviceProvider, ContextType contextType)
    {
        switch (contextType)
        {
            case ContextType.SchoolPilot:
                var schoolPilotContext = serviceProvider.GetRequiredService<ReadWriteSchoolPilotContext>();
                schoolPilotContext.Database.Migrate();
                break;
            case ContextType.Lookup:
                var schoolPilotLookupContext = serviceProvider.GetRequiredService<ReadWriteSchoolPilotLookupContext>();
                schoolPilotLookupContext.Database.Migrate();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDatabaseConnections(hostContext.Configuration);
            });

    public class Options
    {
        [Option('e', "enabled", Required = true, HelpText = "Enable Migrations to run.")]
        public bool Enabled { get; set; }

        [Option('c', "contextType", Required = true, HelpText = "Set which db context will have their migrations ran. (Palmfit = 1, Lookup = 2)")]
        public ContextType DbContextType { get; set; }
    }
}

public enum ContextType
{
    SchoolPilot = 1,
    Lookup = 2
}
