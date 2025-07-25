

using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Context;

namespace SchoolPilot.Infrastructure.Queries.Accounts
{
    public static class GetUserByLoginId
    {
        public class Query : IRequest<Result>
        {
            public Guid LoginId { get; set; }
        }

        [UsedImplicitly]
        public class Result
        {
            public Guid Id { get; set; }

            public Guid AccountId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public Guid? LoginId { get; set; }

            //public bool IsServiceUser { get; set; }

            public DateTime? LastValidated { get; set; }

            public DateTime? CreatedOn { get; set; }

            public DateTime? ModifiedOn { get; set; }
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
                var user = await _readContext.Users.FirstOrDefaultAsync(x => x.LoginId == message.LoginId);

                if (user == null || user.Id == Guid.Empty)
                {
                    return new Result();
                }

                var accountId = await _readContext.UserAffiliations.Where(x => x.UserId == user.Id).Select(x => x.AccountId).FirstOrDefaultAsync();

                return new Result
                {
                    Id = user.Id,
                    AccountId = accountId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    LoginId = user.LoginId,
                    LastValidated = user.LastValidated,
                    CreatedOn = user.CreatedOn,
                    ModifiedOn = user.ModifiedOn
                };
            }
        }
    }
}
