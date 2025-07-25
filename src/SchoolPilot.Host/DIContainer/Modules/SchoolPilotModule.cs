

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPilot.Common.Helpers;
using SchoolPilot.Infrastructure.Helpers;
using SchoolPilot.Print;
using SchoolPilot.Print.Clients;
using SchoolPilot.Print.Interfaces;

namespace SchoolPilot.Host.DIContainer.Modules
{
    public static class SchoolPilotModule
    {
        public static IServiceCollection Load(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<IAppCache, CachingService>();
            services.AddSingleton<IPrintEngine, PrintEngine>();
            services.AddSingleton<IEnumEngine>(_ => new EnumEngine());
            services.AddSingleton<IPermissionMapper>(_ => new PermissionMapper());
            //services.AddSingleton<IAndroidNotificationBroker, AndroidNotificationBroker>();
            //services.AddSingleton<IAppleNotificationBroker, AppleNotificationBroker>();

            var exportServiceApiUrl = configuration["ExportServiceApiUrlFormat"] + "api/pdf/multiform";
            services.AddSingleton<IExportApiClient>(_ => new ExportApiClient(new HttpClient(), exportServiceApiUrl));

            // Auto-register other services
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName?.StartsWith("SchoolPilot") == true);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => !new[]
                {
                    typeof(PrintEngine),
                    typeof(ExportApiClient),
                    //typeof(PharmacyIntegrationHttpClient),
                    //typeof(AndroidNotificationBroker),
                    //typeof(AppleNotificationBroker),
                    typeof(EnumEngine),
                    //typeof(NotificationServiceSettings),
                    typeof(PermissionMapper)
                }.Contains(t));

                foreach (var type in types)
                {
                    var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
                    if (interfaceType != null)
                    {
                        services.AddTransient(interfaceType, type);
                    }
                }
            }

            //services.AddSingleton<IIdentityClientTokenManager>(_ =>
            //new IdentityClientTokenManager(
            //    configuration["Oidc.AuthorityUrl"] + "/connect/token",
            //    configuration["Oidc.ClientId"],
            //    configuration["Oidc.ClientSecret"]));

            //services.AddSingleton<INotificationServiceSettings, NotificationServiceSettings>();
            //services.AddSingleton<IDataCenterService, DataCenterApiClient>();

            return services;
        }
    }
}
