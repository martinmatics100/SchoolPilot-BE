

namespace SchoolPilot.Print.Clients
{
    public class ExportApiClient : IExportApiClient
    {
        private HttpClient _httpClient;
        private string _path;

        public ExportApiClient(HttpClient httpClient, string path)
        {
            _path = path;
            _httpClient = httpClient;
        }

        public async Task<byte[]> GetPdfFromHtml(string html, Dictionary<string, string> settings)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(html), "html");
            foreach (var setting in settings)
            {
                content.Add(new StringContent(setting.Value), setting.Key);
            }

            var response = await _httpClient.PostAsync(_path, content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var stream = new MemoryStream();
            await response.Content.CopyToAsync(stream);
            stream.Flush();
            stream.Position = 0;

            return stream.ToArray();
        }
    }
}
