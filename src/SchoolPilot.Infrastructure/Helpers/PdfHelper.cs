
using System.IO;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;

namespace SchoolPilot.Infrastructure.Helpers
{
    public static class PdfHelper
    {
        //This seems to be the standard for PDF tools
        private const int PixelsPerInch = 72;

        /// <summary>
        /// Takes in a pdf and a stream for a picture and overlays the picture onto the pdf. Distance to top and the height
        /// will need to be manually tested to find the best values per use case.
        /// </summary>
        /// <param name="existingPdf"></param>
        /// <param name="logoStream"></param>
        /// <param name="distanceToTop"></param>
        /// <param name="height"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static byte[] AddImageToPdf(byte[] existingPdf, Stream logoStream, float distanceToTop, float height, int page = 0)
        {
            Stream pdfStream = new MemoryStream(existingPdf);
            var pdf = PdfReader.Open(pdfStream);

            var gfx = XGraphics.FromPdfPage(pdf.Pages[page]);
            var xImage = XImage.FromStream(() => logoStream);
            //This puts the image in the top right, lined up on the left edge with the address below and the top edge with the invoice table next to it
            gfx.DrawImage(xImage, .5 * PixelsPerInch, distanceToTop * PixelsPerInch, 2.5 * PixelsPerInch, height * PixelsPerInch);

            var saveStream = new MemoryStream();
            pdf.Save(saveStream, false);
            return saveStream.ToArray();
        }
    }
}
