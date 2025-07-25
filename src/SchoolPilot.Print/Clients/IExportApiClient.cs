

namespace SchoolPilot.Print.Clients
{
    public interface IExportApiClient
    {
        Task<byte[]> GetPdfFromHtml(string html, Dictionary<string, string> settings);
    }
}
