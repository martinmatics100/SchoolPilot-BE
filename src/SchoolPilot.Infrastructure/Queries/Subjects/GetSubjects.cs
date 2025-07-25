

using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Business.BasicResults;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.Schools;
using System.Net;

namespace SchoolPilot.Infrastructure.Queries.Subjects
{
    public static class GetSubjects
    {
        public class Query : IRequest<Result>
        {
            public Guid AccountId { get; set; }
        }

        public class Result : IActionResult
        {
            public HttpStatusCode Status { get; set; }

            public string ErrorMessage { get; set; }

            public List<SubjectModel> Subjects { get; set; }
        }

        public class SubjectModel
        {
            public Guid Id { get; set; }
            public string SubjectName { get; set; }
            public string SubjectCode { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;

            public Handler(ReadSchoolPilotContext readContext)
            {
                _readContext = readContext;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var schoolId = await _readContext.Schools
                    .Where(x => x.AccountId == message.AccountId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();

                var subjects = await _readContext.SchoolSubjects
                    .Where(s => s.AccountId == message.AccountId && s.SchoolId == schoolId && s.StaffId != Guid.Empty)
                    .ToListAsync(cancellationToken);

                if (!subjects.Any())
                {
                    return new Result
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "No default subjects found for this account."
                    };
                }

                var subjectDtos = subjects.Select(s => new SubjectModel
                {
                    Id = s.Id,
                    SubjectName = s.SubjectName,
                    SubjectCode = s.SubjectCode,
                }).ToList();

                return new Result
                {
                    Subjects = subjectDtos
                };
            }
        }
    }
}
