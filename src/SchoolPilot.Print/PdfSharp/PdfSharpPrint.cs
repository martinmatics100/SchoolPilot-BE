

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.AcroForms;
using PdfSharpCore.Pdf.IO;

namespace SchoolPilot.Print.PdfSharp
{
    public abstract class PdfSharpPrint
    {
        protected PdfDocument Document { get; set; }


        /// <summary>
        /// Gets the pdf file used for the base of a print.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>

        protected PdfDocument GetBaseDocument(string filePath)
        {
            var pathToSearch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "", "Print", filePath);
            var document = PdfReader.Open(pathToSearch, PdfDocumentOpenMode.Modify);
            SetAppearance(document);
            return document;
        }

        /// <summary>
        /// Sets the NeedAppearances flag in the pdf, so that the acro field values are visible.
        /// </summary>
        /// <param name="document"></param>
        protected void SetAppearance(PdfDocument document)
        {
            if (document.AcroForm.Elements.ContainsKey("/NeedAppearances") == false)
            {
                document.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));
            }
            else
            {
                document.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
            }
        }

        /// <summary>
        /// Sets the acro field's value based on the key and document passed in.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fieldKey"></param>
        /// <param name="value"></param>
        protected void SetTextField(PdfDocument document, string fieldKey, string value)
        {
            var field = (PdfTextField)document.AcroForm.Fields[fieldKey];
            field.Value = new PdfString(value);
            if (field.Elements.ContainsKey("/DA") == false)
            {
                field.Elements.Add("/DA", new PdfString("/Helv 0 Tf 0 g"));
            }
            else
            {
                field.Elements["/DA"] = new PdfString("/Helv 0 Tf 0 g");
            }
        }

        /// <summary>
        /// Sets the acro field's value based on the key and document passed in.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fieldKey"></param>
        /// <param name="value"></param>
        /// <param name="appearance">Format: "/{Font} {FontSize} Tf 0 g". Font size 0 is auto sizing. 0 g sets no greyscale, 0 0 0 rg would be for RGB color formatting.</param>
        protected void SetTextField(PdfDocument document, string fieldKey, string value, string appearance)
        {
            var field = (PdfTextField)document.AcroForm.Fields[fieldKey];
            field.Value = new PdfString(value);
            if (field.Elements.ContainsKey("/DA") == false)
            {
                field.Elements.Add("/DA", new PdfString(appearance));
            }
            else
            {
                field.Elements["/DA"] = new PdfString(appearance);
            }
        }

        /// <summary>
        /// Sets and renames the acro field's value based on the key and document passed in.
        /// The renaming allows the field to be set even if exists on more than one page.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="fieldKey"></param>
        /// <param name="newFieldName"></param>
        /// <param name="value"></param>
        protected void SetAndRenameTextField(PdfDocument document, string fieldKey, string newFieldName, string value)
        {
            var field = (PdfTextField)document.AcroForm.Fields[fieldKey];
            field.Elements.SetString("/T", newFieldName);
            field.Value = new PdfString(value);
            if (field.Elements.ContainsKey("/DA") == false)
            {
                field.Elements.Add("/DA", new PdfString("/Helv 0 Tf 0 g"));
            }
            else
            {
                field.Elements["/DA"] = new PdfString("/Helv 0 Tf 0 g");
            }
        }

        /// <summary>
        /// Extracts the page from a pdf file by save it to a stream and reopening it in import mode.
        /// This is due to the fact that pdfs in modify mode cannot have a page taken out of it and put into another pdf.
        /// </summary>
        /// <param name="cloneDocument"></param>
        protected void ExtractPage(PdfDocument cloneDocument)
        {
            MemoryStream stream = new MemoryStream();
            cloneDocument.Save(stream, false);
            var importedChanges = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            Document.AddPage(importedChanges.Pages[0]);
        }

        /// <summary>
        /// Clones the entirety of the pdf, so that it can be used as a template for additional pages.
        /// The clone method that exists within PdfDocument, is a shallow clone, and does not include the acroform.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        protected PdfDocument DeepClone(PdfDocument document)
        {
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            var cloneDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
            SetAppearance(cloneDocument);
            return cloneDocument;
        }

        /// <summary>
        /// Meant to be overwritten by the children, so they can handle creating the pdf file.
        /// </summary>
        protected abstract void SetupPdf();

        /// <summary>
        /// Gets the byte[] of the pdf file.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            SetupPdf();
            MemoryStream stream = new MemoryStream();
            Document.Save(stream, false);
            return stream.ToArray();
        }

        protected byte[] MergePdf(List<byte[]> listOfPdfBytes, string consolidateFileName)
        {
            var mergedPdf = new PdfDocument();

            foreach (var pdfBytes in listOfPdfBytes)
            {
                if (IsPdfValid(pdfBytes))
                {
                    using (var stream = new MemoryStream(pdfBytes))
                    {

                        var pdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

                        foreach (var page in pdf.Pages)
                        {
                            mergedPdf.AddPage(page);
                        }
                    }
                }
            }

            mergedPdf.Save(consolidateFileName);

            byte[] mergedPdfBytes;
            using (var stream = new MemoryStream())
            {
                mergedPdf.Save(stream, false);
                mergedPdfBytes = stream.ToArray();
            }

            return mergedPdfBytes;
        }

        public bool IsPdfValid(byte[] pdfBytes)
        {
            try
            {
                using (var pdfDocument = PdfReader.Open(new MemoryStream(pdfBytes), PdfDocumentOpenMode.Import))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
