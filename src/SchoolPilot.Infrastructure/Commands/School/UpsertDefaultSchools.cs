

using MediatR;
using NLog;
using SchoolPilot.Common.Attributes;
using Newtonsoft.Json;
using JetBrains.Annotations;
using System.Net;
using SchoolPilot.Business.BasicResults;
using SchoolPilot.Data.Context;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data;
using SchoolPilot.Data.Extensions;
using SchoolPilot.Data.Helpers;

namespace SchoolPilot.Infrastructure.Commands.School
{
    public static class UpsertDefaultSchools
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Command : IRequest<Result>
        {
            [JsonIgnore]
            public Guid? AccountId { get; set; }

            [JsonIgnore]
            public Guid? UserId { get; set; }

            [RequireNonDefault]
            public List<DefaultSchoolModel> DefaultSchools { get; set; }
        }


        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class DefaultSchoolModel
        {
            public Guid Id { get; set; }

            public bool IsDefault { get; set; }
        }

        public class Result : IActionResult
        {
            public HttpStatusCode Status { get; set; }

            public string ErrorMessage { get; set; }
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
                var result = new Result();

                var updatedSchoolIds = message.DefaultSchools.ToDictionary(k => k.Id, v => v.IsDefault);

                var existingDefaultSchools = await (from defaultSchool in _readWriteContext.UserDefaultSchools
                                                      where defaultSchool.UserId == message.UserId
                                                            && defaultSchool.AccountId == message.AccountId
                                                      select defaultSchool).ToListAsync();

                foreach (var school in existingDefaultSchools)
                {
                    if (updatedSchoolIds.TryGetValue(school.SchoolId, out var isDefault) && !isDefault)
                    {
                        _readWriteContext.UserDefaultSchools.Remove(school);
                    }
                }

                var existingSchoolIds = existingDefaultSchools.ToHashSet(p => p.SchoolId);

                foreach (var updatedSchool in message.DefaultSchools)
                {
                    if (!existingSchoolIds.Contains(updatedSchool.Id) && updatedSchool.IsDefault)
                    {
                        _readWriteContext.UserDefaultSchools.Add(new UserDefaultSchool
                        {
                            Id = SequentialGuid.Create(),
                            UserId = message.UserId.GetValueOrDefault(),
                            AccountId = message.AccountId.GetValueOrDefault(),
                            SchoolId = updatedSchool.Id
                        });
                    }
                }

                try
                {
                    await _readWriteContext.SaveChangesAsync();
                    result.Status = HttpStatusCode.NoContent;
                    return result;
                }
                catch (DbEntityValidationException e)
                {
                    result.Status = HttpStatusCode.BadRequest;
                    Logger.Error(e, e.Format());
                }

                result.ErrorMessage = "Providers could not be saved.";
                return result;
            }
        }
    }
}
