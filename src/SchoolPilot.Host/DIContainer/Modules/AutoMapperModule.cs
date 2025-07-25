

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SchoolPilot.Host.DIContainer.Modules
{
    public static class AutoMapperModule
    {
        public static void AddAutoMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(
                Assembly.Load("SchoolPilot.Infrastructure"),
                Assembly.Load("SchoolPilot.Business"),
                Assembly.GetExecutingAssembly()
            );
        }
    }
}
