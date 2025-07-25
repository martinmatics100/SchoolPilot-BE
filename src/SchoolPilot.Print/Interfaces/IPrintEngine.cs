

using SchoolPilot.Print.Model;

namespace SchoolPilot.Print.Interfaces
{
    public interface IPrintEngine
    {
        string GenerateBodyHtml<T>(PrintSchema<T> schema, string? templateName = null);

        string GenerateHeaderHtml(BasicPrintHeader schema);

        string GenerateFooterHtml(FooterInfo footer);
    }
}
