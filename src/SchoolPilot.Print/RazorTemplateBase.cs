

using RazorEngine.Templating;
using RazorEngine.Text;

namespace SchoolPilot.Print
{
    public class RazorTemplateBase : TemplateBase
    {
        public IEncodedString EmbedCss(string path)
        {
            var pathToSearch = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.RelativeSearchPath ?? "", path);
            return Raw("<style>" + File.ReadAllText(pathToSearch) + "</style>");
        }
    }
}
