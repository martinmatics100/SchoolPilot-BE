

using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Infrastructure.Helpers;

namespace SchoolPilot.Infrastructure.Queries.Users
{
    public static class GetAllPermissions
    {
        public class Query : IRequest<Result>
        {
            [JsonIgnore]
            public Guid AccountId { get; set; }

            public Guid? UserId { get; set; }
        }

        [UsedImplicitly]
        public class Result : List<Model>
        {
        }

        [UsedImplicitly]
        public class Model
        {
            public string Resource { get; set; }

            public int ResourceType { get; set; }

            public int Action { get; set; }

            public bool? Value { get; set; }
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
            private readonly IMapper _mapper;
            private readonly IPermissionMapper _permissionMapper;

            public Handler(ReadSchoolPilotContext readContext, IMapper mapper, IPermissionMapper permissionMapper)
            {
                _readContext = readContext;
                _mapper = mapper;
                _permissionMapper = permissionMapper;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var permissionList = new List<Model>();
                var setPermissions = new Dictionary<KeyModel, Model>();
                if (message.UserId.HasValue)
                {
                    var userAffiliationId = await (from userAffiliation in _readContext.UserAffiliations
                                                   where userAffiliation.AccountId == message.AccountId
                                                         && userAffiliation.UserId == message.UserId
                                                         && !userAffiliation.IsDeprecated
                                                   select userAffiliation.Id).FirstOrDefaultAsync();

                    if (userAffiliationId == Guid.Empty)
                    {
                        userAffiliationId = message.UserId.Value;
                    }

                    var allUserPermissions = await (from userPrinciple in _readContext.UserPrinciples
                                                    where userPrinciple.IsActive
                                                    join principle in _readContext.Principles
                                                        on
                                                        new { userPrinciple.PrincipleId, userPrinciple.UserAffiliationId, userPrinciple.AccountId }
                                                        equals
                                                        new { PrincipleId = principle.Id, UserAffiliationId = userAffiliationId, message.AccountId }
                                                    from permission in _readContext.Permissions
                                                    where permission.PrincipleId == principle.Id
                                                    select new { permission, principle.PermissionType }).ToListAsync();

                    setPermissions = allUserPermissions.Where(x => x.PermissionType == PermissionAccessType.Regular
                                                                                        || x.PermissionType == PermissionAccessType.Readonly)
                                                .GroupBy(x => new { x.permission.Resource, x.permission.ResourceType, x.permission.Action })
                                                .Select(g => new Model
                                                {
                                                    Resource = g.Key.Resource,
                                                    ResourceType = g.Key.ResourceType,
                                                    Action = g.Key.Action,
                                                    Value = g.Max(s => s.permission.Value) == PermissionValue.Granted
                                                }).ToDictionary(x => new KeyModel(x.Resource, x.Action));

                    var userEmergencyPermissions = allUserPermissions.Where(x => x.PermissionType == PermissionAccessType.Emergency).Select(g => new Model
                    {
                        Resource = g.permission.Resource,
                        ResourceType = g.permission.ResourceType,
                        Action = g.permission.Action,
                        Value = g.permission.Value == PermissionValue.Granted
                    }).ToDictionary(x => new KeyModel(x.Resource, x.Action));

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
                }

                var fullPermissionSet = _permissionMapper.GetPermissionList();
                foreach (var permission in fullPermissionSet)
                {
                    var model = _mapper.Map<Model>(permission);
                    if (setPermissions.TryGetValue(new KeyModel(model.Resource, model.Action), out var setPermission))
                    {
                        model.Value = setPermission.Value;
                    }
                    else
                    {
                        //defaulting any fields not set to the equivalent of PermissionValue.Denied (which is inverted due to checking if granted above)
                        model.Value = false;
                    }

                    permissionList.Add(model);
                }

                return _mapper.Map<Result>(permissionList);
            }
        }
    }
}
