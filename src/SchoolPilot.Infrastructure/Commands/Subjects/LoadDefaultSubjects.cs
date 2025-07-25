

using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;
using SchoolPilot.Business.BasicResults;
using SchoolPilot.Common.Constants;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Common.Model;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.Schools;
using SchoolPilot.Data.Helpers;
using System.Net;
using System.Reactive.Subjects;

namespace SchoolPilot.Infrastructure.Commands.Subjects
{
    public static class LoadDefaultSubjects
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Command : IRequest<Result>
        {
            public Guid AccountId { get; set; }
        }

        public class  Result : IActionResult
        {
            public HttpStatusCode Status { get; set; }

            public string ErrorMessage { get; set; }

            public Result(HttpStatusCode status)
            {
                Status = status;
            }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ReadWriteSchoolPilotContext _readWriteContext;

            public Handler(ReadWriteSchoolPilotContext readWriteContext)
            {
                _readWriteContext = readWriteContext;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
               var existingSubjects = await _readWriteContext.SchoolSubjects
                    .Where(s => s.AccountId == message.AccountId).ToListAsync(cancellationToken);

                var schoolId = await _readWriteContext.Schools.Where(x => x.AccountId == message.AccountId).Select(x => x.Id).SingleOrDefaultAsync(cancellationToken);

                if (schoolId == Guid.Empty)
                {
                    Logger.Warn("School not found for account {AccountId}", message.AccountId);
                    return new Result(HttpStatusCode.NotFound);
                }

                if (existingSubjects.Count >= SubjectDefinitionsConstants.All.Count)
                {
                    Logger.Info("All default subjects already exist for school {SchoolId}", message.AccountId);
                    return new Result(HttpStatusCode.NotFound);
                }

                var existingName = existingSubjects.Select(s => s.SubjectName).ToHashset();

                var subjectsToAdd = new List<SchoolSubject>();

                foreach (var def in SubjectDefinitionsConstants.All)
                {
                    if (!existingName.Contains(def.Code))
                    {
                        subjectsToAdd.Add(new SchoolSubject
                        {
                            Id = SequentialGuid.Create(),
                            SchoolId = schoolId,
                            SubjectName = def.Name,
                            SubjectCode = def.Code,
                            AccountId = message.AccountId,
                            Level = def.Levels.ToList(),
                            Category = def.Category.ToList(),
                        });
                    }
                }

                await _readWriteContext.SchoolSubjects.AddRangeAsync(subjectsToAdd, cancellationToken);

                try
                {
                    if(await _readWriteContext.SaveChangesAsync(cancellationToken) > 0)
                    {
                        Logger.Info("Added {Count} default subjects for school {SchoolId}", subjectsToAdd.Count, message.AccountId);
                        return new Result(HttpStatusCode.NoContent);
                    }

                    Logger.Warn("No subjects were saved to the database");
                    return new Result(HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to load default subjects for school {SchoolId}", message.AccountId);
                    return new Result(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}
