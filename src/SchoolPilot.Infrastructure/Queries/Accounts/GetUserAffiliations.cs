

using AutoMapper;
using AutoMapper.QueryableExtensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Infrastructure.Queries.Accounts
{
    public static class GetUserAffiliations
    {
        public class Query : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public Guid? AccountId { get; set; }
        }

        public class Result
        {
            public ICollection<Affiliation> Affiliations { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class Affiliation
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }

            public Guid AccountId { get; set; }
            public UserStatus Status { get; set; }

            public DateTime? CreatedOn { get; set; }

            public DateTime? ModifiedOn { get; set; }

            public bool IsSupportUser { get; set; }

            public bool AllowEmergencyAccessRequest { get; set; }

            public bool AllowReadOnlyAccess { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;
            private readonly IMapper _mapper;

            public Handler(ReadSchoolPilotContext context, IMapper mapper)
            {
                _readContext = context;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var dbResult = await _readContext.UserAffiliations
                    .Where(affliation => affliation.UserId == message.UserId
                    && (message.AccountId != null || affliation.AccountId == message.AccountId)
                    && !affliation.IsDeprecated)
                    .ProjectTo<Affiliation>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new Result
                {
                    Affiliations = dbResult
                };
            }
        }
    }
}
