using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Infrastructure.Queries.General
{
    public static class GetCountries
    {
        public class Query : IRequest<List<Result>>
        {
        }

        public class Result
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Region { get; set; }
            public string PhoneCode { get; set; }
            public string Currency { get; set; }
            public string SubRegion { get; set; }
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
                var countries = await _readLookupContext.Countries
                    .OrderBy(c => c.Name)
                    .Select(c => new Result
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Code,
                        PhoneCode = c.PhoneCode,
                        Currency = c.Currency,
                        Region = c.Region,
                        SubRegion = c.Subregion
                    })
                    .ToListAsync(cancellationToken);

                return countries;
            }
        }
    }
}
