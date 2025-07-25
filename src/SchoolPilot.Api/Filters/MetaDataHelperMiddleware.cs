

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Diagnostics;
using System.Text;

namespace SchoolPilot.Api.Filters
{
    public class MetaDataHelperMiddleware
    {
        private const string Delimiter = ":";
        private static readonly Logger HttpLogger = LogManager.GetLogger("HttpLogger");
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly bool _httpLoggingEnabled;

        public MetaDataHelperMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
            _httpLoggingEnabled = _config["App.Environment"] == "Production";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var actionId = Guid.NewGuid().ToString("N");
            var previousId = context.Request.Headers["X-SchoolPilot-ActionId"].FirstOrDefault();
            var traceId = $"{previousId}{Delimiter}{actionId}";

            using (MappedDiagnosticsLogicalContext.SetScoped("ActionId", actionId))
            using (MappedDiagnosticsLogicalContext.SetScoped("TraceGlob", traceId))
            {
                if (_httpLoggingEnabled)
                {
                    LogRequest(context);
                }

                var timer = Stopwatch.StartNew();
                context.Response.OnStarting(() =>
                {
                    AddResponseHeaders(context, actionId, timer);
                    return Task.CompletedTask;
                });

                await _next(context);
                timer.Stop();

                if (_httpLoggingEnabled && context.Response.StatusCode >= 400)
                {
                    LogFailedResponse(context);
                }
            }
        }

        private void LogRequest(HttpContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{context.Request.Method} {context.Request.Path} HTTP/{context.Request.Protocol}");
            sb.AppendLine(string.Join("\n", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}")));

            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                sb.AppendLine("\nBody: [logged separately]");
            }

            HttpLogger.Debug(sb.ToString());
        }

        private void AddResponseHeaders(HttpContext context, string actionId, Stopwatch timer)
        {
            context.Response.Headers.Append("X-SchoolPilot-ActionId", actionId);
            context.Response.Headers.Append("X-SchoolPilot-ReleaseVersion", _config["App.Release"]);
            context.Response.Headers.Append("X-SchoolPilot-Environment", _config["App.Environment"]);
            context.Response.Headers.Append("X-SchoolPilot-Duration", timer.ElapsedMilliseconds.ToString());
        }

        private void LogFailedResponse(HttpContext context)
        {
            // Check if status code indicates failure (400-599)
            if (context.Response.StatusCode >= 400)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Failed request - {context.Response.StatusCode}");
                sb.AppendLine(string.Join("\n", context.Response.Headers.Select(h => $"{h.Key}: {h.Value}")));
                HttpLogger.Debug(sb.ToString());
            }
        }
    }
}
