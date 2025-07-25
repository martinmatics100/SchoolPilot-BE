

using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Infrastructure.Queries.Users
{
    public static class GetUserAffiliationRestrictionsById
    {
        public class Query : IRequest<Result>
        {
            public Guid UserAffiliationId { get; set; }
        }

        [UsedImplicitly]
        public class Result
        {
            public int? EarliestLoginTimeInMinutes { get; set; }

            public int? AutomaticLogoutTimeInMinutes { get; set; }

            public bool AllowWeekendAccess { get; set; }

            public short TimeZoneOffset { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;
            private readonly IMapper _mapper;

            public Handler(ReadSchoolPilotContext readContext, IMapper mapper)
            {
                _readContext = readContext;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var userAff = await _readContext.UserAffiliations.SingleOrDefaultAsync(x => x.Id == message.UserAffiliationId && !x.IsDeprecated);
                var result = _mapper.Map<Result>(userAff);
                return result;
            }
        }
    }
}
