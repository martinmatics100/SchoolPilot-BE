

using Refit;

namespace SchoolPilot.Business.Interfaces
{
    [Headers("Authorization: Bearer")]
    public interface IAssetApi
    {
        [Get("/api/v1/assets/{assetId}")]
        Task<GetAssetResponse> GetAsync(Guid assetId);

        [Post("/api/v1/assets")]
        Task<UploadAssetResponse> Post([Body] UploadAssetRequest command, [Header("X-Bucket-Id")] Guid bucketId);
    }

    public interface IAssetStorageApi
    {
        [Get("/api/v1/assets")]
        Task<HttpResponseMessage> GetAsync([Header("Authorization")] string auth);

        [Post("/api/v1/assets")]
        Task<HttpResponseMessage> PostAsync([Body] MultipartFormDataContent file, [Header("Authorization")] string auth, [Header("X-Bucket-Id")] Guid bucketId);
    }


    /// <summary>
    /// The response from the assetapi for getting an asset.
    /// </summary>
    public class GetAssetResponse
    {
        public string Token { get; set; }

        public string FileType { get; set; }

        public string FileName { get; set; }
    }

    public class UploadAssetRequest
    {
        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string StorageClass { get; set; }
    }

    public class UploadAssetResponse
    {
        public string Token { get; set; }

        public Guid AssetId { get; set; }
    }

    public static class ApiConstants
    {
        public const string StorageTokenRoute = "/api/v1/assets?token=";
    }

}
