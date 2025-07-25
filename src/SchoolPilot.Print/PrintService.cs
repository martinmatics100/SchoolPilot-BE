

using SchoolPilot.Print.Attributes;
using SchoolPilot.Print.Clients;
using SchoolPilot.Print.Interfaces;
using SchoolPilot.Print.Model;
using System.Reflection;

namespace SchoolPilot.Print
{
    public class PrintService : IPrintService
    {
        private IPrintEngine _printEngine;
        private IExportApiClient _exportApiClient;

        public PrintService(IPrintEngine engine, IExportApiClient exportApiClient)
        {
            _printEngine = engine;
            _exportApiClient = exportApiClient;
        }

        public async Task<byte[]> GeneratePdf<T>(PrintSchema<T> request, string? title, string? templateFileName, bool isLandscape = false)
        {
            request.HeaderInfo.Title = request.HeaderInfo.Title ?? title;
            var bodyHtml = _printEngine.GenerateBodyHtml(request, templateFileName);
            var headerHtml = _printEngine.GenerateHeaderHtml(request.HeaderInfo);
            var footerHtml = _printEngine.GenerateFooterHtml(request.Footer);


            // settings for wkhtmltopdf to set up header/footer properly
            // including sending header html seperately as header-html-content
            var settings = new Dictionary<string, string>();
            settings.Add("header-html-content", headerHtml);
            settings.Add("footer-html-content", footerHtml);
            settings.Add("margin-top", request.HeaderInfo.MarginTop.ToString());
            settings.Add("margin-left", "10");
            settings.Add("margin-bottom","2");
            settings.Add("margin-right", "10");

            if (isLandscape)
            {
                settings.Add("orientation", "landscape");
            }

            return await _exportApiClient.GetPdfFromHtml(bodyHtml, settings);
        }

        public async Task<byte[]> GeneratePdf<T>(PrintSchema<T> request, string templateFileName = null, bool isLandscape = false)
        {
            var title = typeof(T)?.GetCustomAttribute<PrintTitleAttribute>()?.Title;
            return await GeneratePdf(request, title, templateFileName, isLandscape);
        }
    }
}
