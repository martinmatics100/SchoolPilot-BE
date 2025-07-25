

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;

namespace SchoolPilot.Host.DIContainer.Modules
{
    public static class MediatRModule
    {
        public static void AddMediatRServices(this IServiceCollection services)
        {
            var assemblyNames = new[]
            {
                "SchoolPilot.Api",
                //"SchoolPilot.Identity",
                "SchoolPilot.Data",
                "SchoolPilot.Business",
                "SchoolPilot.Infrastructure"
            };

            var assemblies = new List<Assembly>();
            foreach (var name in assemblyNames)
            {
                try
                {
                    assemblies.Add(Assembly.Load(name));
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Assembly not found: {name}");
                }
            }

            services.AddMediatR(assemblies);
        }
    }
}
