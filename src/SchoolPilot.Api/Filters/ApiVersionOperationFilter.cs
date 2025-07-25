

using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SchoolPilot.Api.Filters
{
    public class ApiVersionOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiVersion = context.ApiDescription.GroupName;
            if (string.IsNullOrEmpty(apiVersion))
                return;

            var versions = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionAttribute>()
                .SelectMany(attr => attr.Versions)
                .Select(v => $"v{v}");

            if (!versions.Contains(apiVersion))
            {
                operation.Deprecated = true; // Marks as deprecated instead of removing it completely
            }
        }
    }
}
