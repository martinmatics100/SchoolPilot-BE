

using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Infrastructure.Queries.Accounts
{
    public static class GetUserDefaultLocationInformation
    {
        public class Query : IRequest<Result>
        {
            public Guid UserId { get; set; }

            public Guid AccountId { get; set; }
        }

        public class Result
        {
            public List<Guid> LocationIds { get; set; }

            public bool IsInReadonlyMode { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;

            public Handler(ReadSchoolPilotContext readContext)
            {
                _readContext = readContext;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = new Result();

                //Semi-hacky solution to determine view mode.
                //should only be 1 user principle active between readonly & regular
                //this prevents FE from having to tell BE state beyond account selection screen
                var principleType = await (from userPrinciple in _readContext.UserPrinciples
                                           join agencyPrinciple in _readContext.Principles on userPrinciple.PrincipleId equals agencyPrinciple.Id
                                           join userAffiliation in _readContext.UserAffiliations on userPrinciple.UserAffiliationId equals userAffiliation.Id
                                           where agencyPrinciple.AccountId == message.AccountId
                                                 && userPrinciple.AccountId == message.AccountId
                                                 && userAffiliation.UserId == message.UserId
                                                 && userPrinciple.IsActive
                                                 && agencyPrinciple.PermissionType != PermissionAccessType.Emergency
                                           select agencyPrinciple.PermissionType)
                    .FirstOrDefaultAsync();

                result.IsInReadonlyMode = principleType == PermissionAccessType.Readonly;

                result.LocationIds = await (from userLoc in _readContext.UserLocations
                                          .Where(w => w.AccountId == message.AccountId && w.UserId == message.UserId)
                                            join agencyLocation in _readContext.SchoolLocations on userLoc.AgencyLocationId equals agencyLocation.Id
                                            join provider in _readContext.Staffs on agencyLocation.SchoolId equals provider.Id
                                            where !agencyLocation.IsDeprecated
                                                  && result.IsInReadonlyMode == agencyLocation.IsReadOnly
                                                  && !provider.IsDeprecated
                                            select userLoc.AgencyLocationId
                   ).ToListAsync();

                return result;
            }
        }
    }
}
