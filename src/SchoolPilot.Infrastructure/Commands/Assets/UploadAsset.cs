using MediatR;
using NLog;
using SchoolPilot.Infrastructure.Helpers.SuperbaseHelper;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SchoolPilot.Business.Model;

namespace SchoolPilot.Infrastructure.Commands.Assets
{
    public static class UploadAsset
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Command : IRequest<Result>
        {
            [Required(ErrorMessage = "File name is required")]
            public string FileName { get; set; }

            public Stream FileStream { get; set; }

            [Required]
            public string ContentType { get; set; }

            [JsonIgnore]
            public Guid AccountId { get; set; }
        }

        public class Result
        {
            public string Url { get; set; }
            public Guid FileId { get; set; } 
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly SupabaseStorageService _storageService;

            public Handler(SupabaseStorageService storageService)
            {
                _storageService = storageService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var uploadResult = await _storageService.UploadFileAsync(new FileUploadRequest
                    {
                        FileName = request.FileName,
                        FileStream = request.FileStream,
                        ContentType = request.ContentType,
                        AccountId = request.AccountId
                    });

                    return new Result
                    {
                        Url = uploadResult.PublicUrl,
                        FileId = uploadResult.FileId,
                    };
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Supabase upload failed");
                    return new Result();
                }
            }
        }
    }
}