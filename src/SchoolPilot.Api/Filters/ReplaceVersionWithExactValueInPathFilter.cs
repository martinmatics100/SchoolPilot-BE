

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace SchoolPilot.Api.Filters
{
    public class ReplaceVersionWithExactValueInPathFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                var newKey = Regex.Replace(path.Key, @"\{version\}", swaggerDoc.Info.Version);
                paths.Add(newKey, path.Value);
            }
            swaggerDoc.Paths = paths;
        }
    }
}
