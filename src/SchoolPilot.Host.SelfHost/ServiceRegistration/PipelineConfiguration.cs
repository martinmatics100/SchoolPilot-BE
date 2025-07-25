using Hangfire;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolPilot.Api.Filters;
using SchoolPilot.Host.Hangfire;

namespace SchoolPilot.Host.SelfHost.ServiceRegistration
{
    public static class PipelineConfiguration
    {
        public static void ConfigurePipeline(WebApplication app)
        {
            if (app.Environment.IsProduction())
            {
                app.UseMiddleware<MetaDataHelperMiddleware>();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPilot v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "SchoolPilot v2");
                });
            }

            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();

            app.UseAuthentication()
                .UseAuthorization();

            app.MapControllers();
        }

    }
}
