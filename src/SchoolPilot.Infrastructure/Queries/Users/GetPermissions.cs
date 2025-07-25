

using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.Users;

namespace SchoolPilot.Infrastructure.Queries.Users
{
    public static class GetPermissions
    {
        public class Query : IRequest<Result>
        {
            public Guid UserAffiliationId { get; set; }

            public Guid AccountId { get; set; }
        }

        public class Result
        {
            public IList<Model> Permissions { get; set; }
        }

        public class Model
        {
            public string Resource { get; set; }

            public ResourceType ResourceType { get; set; }

            public PermissionActions Action { get; set; }

            public PermissionValue Value { get; set; }
        }

        private class KeyModel
        {
            public KeyModel(string resource, int action)
            {
                Resource = resource;
                Action = action;
            }

            public string Resource { get; }

            public int Action { get; }

            public override bool Equals(object obj)
            {
                var model = obj as KeyModel;
                return model != null &&
                       Resource == model.Resource &&
                       Action == model.Action;
            }

            public override int GetHashCode()
            {
                var hashCode = -1048429188;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Resource);
                hashCode = hashCode * -1521134295 + Action.GetHashCode();
                return hashCode;
            }
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
                var allUserPermissions = await (from userPrinciple in _readContext.UserPrinciples
                                                where userPrinciple.IsActive
                                                join principle in _readContext.Principles
                                                    on
                                                    new { userPrinciple.PrincipleId, userPrinciple.UserAffiliationId, userPrinciple.AccountId }
                                                    equals
                                                    new { PrincipleId = principle.Id, message.UserAffiliationId, message.AccountId }
                                                from permission in _readContext.Permissions
                                                where permission.PrincipleId == principle.Id
                                                select new { permission, principle.PermissionType }).ToListAsync();

                var setPermissions = allUserPermissions.Where(x => x.PermissionType == PermissionAccessType.Regular
                                                                                                             || x.PermissionType == PermissionAccessType.Readonly)
                    .GroupBy(x => new { x.permission.Resource, x.permission.ResourceType, x.permission.Action })
                    .Select(g => new Model
                    {
                        Resource = g.Key.Resource,
                        ResourceType = (ResourceType)g.Key.ResourceType,
                        Action = (PermissionActions)g.Key.Action,
                        Value = g.Max(s => s.permission.Value)
                    }).ToDictionary(x => new KeyModel(x.Resource, (int)x.Action));

                var userEmergencyPermissions = allUserPermissions.Where(x => x.PermissionType == PermissionAccessType.Emergency).Select(g => new Model
                {
                    Resource = g.permission.Resource,
                    ResourceType = (ResourceType)g.permission.ResourceType,
                    Action = (PermissionActions)g.permission.Action,
                    Value = g.permission.Value
                }).ToDictionary(x => new KeyModel(x.Resource, (int)x.Action));

                foreach (var ep in userEmergencyPermissions)
                {
                    if (setPermissions.ContainsKey(ep.Key))
                    {
                        setPermissions[ep.Key] = ep.Value;
                    }
                    else
                    {
                        setPermissions.Add(ep.Key, ep.Value);
                    }
                }

                return new Result
                {
                    Permissions = setPermissions.Values.ToList()
                };
            }
        }
    }
}
