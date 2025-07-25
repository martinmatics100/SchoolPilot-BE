

using MediatR;
using NodaTime;
using SchoolPilot.Common.Interfaces;
using Newtonsoft.Json;
using SchoolPilot.Common.Enums;
using JetBrains.Annotations;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities;
using SchoolPilot.Data.Helpers;

namespace SchoolPilot.Infrastructure.Queries.Accounts
{
    public static class GetAccounts
    {
        public class Query : IRequest<Result>, IPagedRequest
        {
            [JsonIgnore]
            public Guid? UserId { get; set; }

            public int Page { get; set; }

            public int PageLength { get; set; }
        }

        public class Result : PagedResult<Model>
        {
        }


        public class Model
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public bool Disabled { get; set; }

            public DateTime? CreatedOn { get; set; }

            public DateTime? ModifiedOn { get; set; }

            public DateTime? DisabledOn { get; set; }

            [JsonIgnore]
            public UserStatus Status { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ReadSchoolPilotContext _readContext;
            private readonly IMethodExecutionHelper _executionHelper;

            public Handler(ReadSchoolPilotContext readContext, IMethodExecutionHelper executionHelper)
            {
                _readContext = readContext;
                _executionHelper = executionHelper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                if (!message.UserId.HasValue)
                {
                    return new Result();
                }

                var result = await (from account in _readContext.Accounts
                                    where !account.Flags.HasFlag(AccountMiscFlags.IsDisabled)
                                    from userAffiliation in _readContext.UserAffiliations.Where(x => x.UserId == message.UserId && account.Id == x.AccountId)
                                    where userAffiliation.Status != UserStatus.Inactive
                                    && !userAffiliation.IsDeprecated
                                    orderby account.Name
                                    select new Model
                                    {
                                        Id = account.Id,
                                        Name = account.Name,
                                        Disabled = account.Flags.HasFlag(AccountMiscFlags.IsDisabled),
                                        CreatedOn = account.CreatedOn,
                                        ModifiedOn = account.ModifiedOn,
                                        //DisabledOn = account.DisabledOn
                                        Status = userAffiliation.Status
                                    })
                                    .ToPageResultsAsync<Model, Result>(message);

                return result;
            }
        }
    }
}
