

using SchoolPilot.Business.Model;

namespace SchoolPilot.Business.Interfaces
{
    public interface ISupabaseStorageService
    {
        Task<FileUploadResult> UploadFileAsync(FileUploadRequest request);
        Task<FileDownloadResult> DownloadFileAsync(Guid fileId, Guid accountId);
        Task<string> GetFileUrlAsync(Guid fileId, Guid accountId);
        Task DeleteFileAsync(Guid fileId, Guid accountId);
    }
}
