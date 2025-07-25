using Microsoft.EntityFrameworkCore;
using SchoolPilot.Business.Interfaces;
using SchoolPilot.Business.Model;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Helpers;
using Supabase;
using System.Net.Mime;

namespace SchoolPilot.Infrastructure.Helpers.SuperbaseHelper
{
    public class SupabaseStorageService : ISupabaseStorageService
    {
        private readonly Client _supabase;
        private const string BucketName = "school-pilot-bucket";
        private readonly ReadWriteSchoolPilotContext _readWriteContext;

        public SupabaseStorageService(Client supabase, ReadWriteSchoolPilotContext context)
        {
            _supabase = supabase;
            _readWriteContext = context;
        }

        public async Task<FileUploadResult> UploadFileAsync(FileUploadRequest request)
        {
            await EnsureInitialized();
            var storage = _supabase.Storage.From(BucketName);

            var fileId = SequentialGuid.Create();
            var uniqueFileName = $"{request.AccountId}/{fileId}_{Path.GetFileName(request.FileName)}";

            // Convert stream to byte array for upload
            using var memoryStream = new MemoryStream();
            await request.FileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            await storage.Upload(fileBytes, uniqueFileName);

            var metadata = new StoredFilesMetadata
            {
                Id = fileId,
                AccountId = request.AccountId,
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSize = request.FileStream.Length,
                StoragePath = uniqueFileName,
                PublicUrl = storage.GetPublicUrl(uniqueFileName),
            };

            await _readWriteContext.StoredFilesMetadatas.AddAsync(metadata);

            if(await _readWriteContext.SaveChangesAsync() > 0)
            {
                return new FileUploadResult
                {
                    FileId = fileId,
                    PublicUrl = metadata.PublicUrl,
                };
            }
            else
            {
                throw new Exception("Failed to save file metadata.");
            }
        }

        public async Task<FileDownloadResult> DownloadFileAsync(Guid fileId, Guid accountId)
        {
            await EnsureInitialized();
            var metadata = await _readWriteContext.StoredFilesMetadatas
                .Where(x => x.Id == fileId && x.AccountId == accountId).FirstOrDefaultAsync();

            if(metadata == null)
            {
                throw new UnauthorizedAccessException("File not found or access denied");
            }
            var storage = _supabase.Storage.From(BucketName);
            var bytes = await storage.DownloadPublicFile(metadata.StoragePath);

            return new FileDownloadResult
            {
                FileStream = new MemoryStream(bytes),
                FileName = metadata.FileName,
                ContentType = metadata.ContentType
            };
        }

        public async Task<string> GetFileUrlAsync(Guid fileId, Guid accountId)
        {
            await EnsureInitialized();

            var metadata = await _readWriteContext.StoredFilesMetadatas
                .Where(x => x.Id == fileId && x.AccountId == accountId).FirstOrDefaultAsync();

            if (metadata == null)
            {
                throw new UnauthorizedAccessException("File not found or access denied");
            }

            return metadata.PublicUrl;
        }

        public async Task DeleteFileAsync(Guid fileId, Guid accountId)
        {
            await EnsureInitialized();

            var metadata = await _readWriteContext.StoredFilesMetadatas
                .Where(x => x.Id == fileId && x.AccountId == accountId).FirstOrDefaultAsync();

            if (metadata == null)
            {
                throw new UnauthorizedAccessException("File not found or access denied");
            }

            var storage = _supabase.Storage.From(BucketName);
            await storage.Remove(metadata.StoragePath);

            metadata.IsDeprecated = true;
            await _readWriteContext.SaveChangesAsync();
        }

        private async Task EnsureInitialized()
        {
            if (_supabase == null)
                throw new InvalidOperationException("Supabase client not initialized");

            await _supabase.InitializeAsync();
        }
    }
}