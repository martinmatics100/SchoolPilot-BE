

using SchoolPilot.Print.Model;

namespace SchoolPilot.Print.Interfaces
{
    public interface IPrintService
    {
        Task<byte[]> GeneratePdf<T>(PrintSchema<T> request, string? templateFileName = null, bool isLandscape = false);

        Task<byte[]> GeneratePdf<T>(PrintSchema<T> request, string title, string templateFileName, bool isLandscape = false);
    }
}
