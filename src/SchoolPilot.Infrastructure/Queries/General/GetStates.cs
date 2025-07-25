
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolPilot.Common.Helpers;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.lookup;

namespace SchoolPilot.Infrastructure.Queries.General
{
    public static class GetStates
    {
        public class Query : IRequest<List<Result>>
        {
            [JsonIgnore]
            public Guid CountryId { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Region { get; set; }
            public bool IsNigerianState { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, List<Result>>
        {
            private readonly ReadSchoolPilotLookupContext _readLookupContext;

            public Handler(ReadSchoolPilotLookupContext readLookupContext)
            {
                _readLookupContext = readLookupContext;
            }

            public async Task<List<Result>> Handle(Query message, CancellationToken cancellationToken)
            {
                var states = await _readLookupContext.States.Where(s => s.CountryId == message.CountryId)
                    .OrderBy(s => s.Name)
                    .Select(s => new Result
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code,
                        Region = s.Region,
                        IsNigerianState = s.IsNigerianState,
                    }).ToListAsync(cancellationToken);

                return states;
            }
        }
    }
}
