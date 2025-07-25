

using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Data.Context;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Infrastructure.Commands.Users
{
    public static class UpdateUserFromToken
    {
        public class Command : IRequest<Result>
        {
            [Required]
            public Guid LoginId { get; set; }

            [StringLength(64)]
            public string FirstName { get; set; }

            [StringLength(64)]
            public string LastName { get; set; }

            [Required]
            public DateTime LastValidated { get; set; }
        }

        [UsedImplicitly]
        public class Result
        {
            public bool UserUpdated { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ReadWriteSchoolPilotContext _readWriteContext;

            public Handler(ReadWriteSchoolPilotContext readContext)
            {
                _readWriteContext = readContext;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await (from u in _readWriteContext.Users
                                  where u.LoginId == command.LoginId
                                  select u).SingleAsync();

                if (user == null || user.LastValidated != null && !(user.LastValidated < command.LastValidated))
                {
                    return new Result { UserUpdated = false };
                }

                user.LastValidated = command.LastValidated;
                if (!string.IsNullOrWhiteSpace(command.FirstName))
                {
                    user.FirstName = command.FirstName;
                }

                if (!string.IsNullOrWhiteSpace(command.LastName))
                {
                    user.LastName = command.LastName;
                }

                await _readWriteContext.SaveChangesAsync();

                return new Result { UserUpdated = true };
            }
        }
    }
}
