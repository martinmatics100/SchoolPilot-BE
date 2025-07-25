

namespace SchoolPilot.Business.Model
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
        public Guid AccountId { get; set; }
    }

    public class FileUploadResult
    {
        public Guid FileId { get; set; }
        public string PublicUrl { get; set; }
    }

    public class FileDownloadResult
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
