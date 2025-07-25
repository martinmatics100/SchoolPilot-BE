using MediatR;
using NLog;
using SchoolPilot.Business.Interfaces;


namespace SchoolPilot.Infrastructure.Commands.Assets
{
    public class DownloadAsset
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Command : IRequest<Result>
        {
            public Guid FileId { get; set; }

            public Guid AccountId { get; set; }
        }

        public class Result
        {
            public Stream FileStream { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ISupabaseStorageService _storageService;

            public Handler(ISupabaseStorageService storageService)
            {
                _storageService = storageService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var stream = await _storageService.DownloadFileAsync(request.FileId, request.AccountId);
                    var fileName = await _storageService.GetFileUrlAsync(request.FileId, request.AccountId);

                    return new Result
                    {
                        FileStream = stream.FileStream,
                        FileName = Path.GetFileName(fileName),
                        ContentType = "application/octet-stream" 
                    };
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Supabase download failed");
                    return new Result();
                }
            }
        }
    }
}
