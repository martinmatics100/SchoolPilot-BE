

using MediatR;
using NLog;
using Newtonsoft.Json;
using System.Net;
using JetBrains.Annotations;
using SchoolPilot.Infrastructure.Helpers;
using SchoolPilot.Data.Context;
using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Infrastructure.Attributes;
using SchoolPilot.Data.Helpers;
using SchoolPilot.Common.Extensions;
using SchoolPilot.Data.Entities.Logs;
using SchoolPilot.Data;
using SchoolPilot.Data.Extensions;

namespace SchoolPilot.Infrastructure.Commands.Users
{
    public static class UpdatePermissions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public class Command : IRequest<Result>
        {
            [JsonIgnore]
            public Guid AccountId { get; set; }

            [JsonIgnore]
            public Guid UserId { get; set; }

            public List<PermissionModel> Permissions { get; set; }

            [JsonIgnore]
            public Guid RequestUserId { get; set; }
        }

        public class PermissionModel
        {
            public string Resource { get; set; }

            public int ResourceType { get; set; }

            public int Action { get; set; }

            public bool? Value { get; set; }
        }

        public class Result
        {
            public Result(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }

            public HttpStatusCode StatusCode { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ReadWriteSchoolPilotContext _readWriteContext;
            private readonly IActivityLogHelper _activityLogHelper;

            public Handler(ReadWriteSchoolPilotContext readWriteContext, IActivityLogHelper activityLogHelper)
            {
                _readWriteContext = readWriteContext;
                _activityLogHelper = activityLogHelper;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                var userAffiliation = await (from userAff in _readWriteContext.UserAffiliations
                                             where userAff.AccountId == message.AccountId
                                                   // This is because the front-end randomly passes both userId and userAffiliationId
                                                   && (userAff.UserId == message.UserId || userAff.Id == message.UserId)
                                                   && !userAff.IsDeprecated
                                             select userAff).FirstOrDefaultAsync();

                if (userAffiliation == null)
                {
                    return new Result(HttpStatusCode.NotFound);
                }

                var userAffiliationId = userAffiliation.Id;

                var permissionChanges = new Dictionary<PermissionChanges, PermissionChanges>();
                var removedPermissions = new List<PermissionChanges>();

                var userPrinciple = await _readWriteContext.UserPrinciples.FirstOrDefaultAsync(x => x.AccountId == message.AccountId && x.UserAffiliationId == userAffiliationId);
                if (userPrinciple == null)
                {
                    var principleToAdd = new Principle
                    {
                        Id = SequentialGuid.Create(),
                        AccountId = message.AccountId
                    };

                    var permissionsToAdd = message.Permissions
                       .Where(x => x.Value.HasValue)
                       .Select(x => new Permission
                       {
                           AccountId = message.AccountId,
                           PrincipleId = principleToAdd.Id,
                           ResourceType = x.ResourceType,
                           Resource = x.Resource,
                           Action = x.Action,
                           Value = x.Value.Value ? PermissionValue.Granted : PermissionValue.Denied
                       }).ToList();

                    var userPrincipleToAdd = new UserPrinciple
                    {
                        AccountId = message.AccountId,
                        PrincipleId = principleToAdd.Id,
                        UserAffiliationId = userAffiliationId,
                        ExpirationDate = DateTimeExtensions.MaxDate,
                        IsActive = true
                    };

                    foreach (var permission in permissionsToAdd)
                    {
                        var newPermission = GeneratePermissionChangesObjectForActivityLogs(permission);
                        permissionChanges.Add(newPermission, null);
                    }

                    _readWriteContext.Permissions.AddRange(permissionsToAdd);
                    _readWriteContext.Principles.Add(principleToAdd);
                    _readWriteContext.UserPrinciples.Add(userPrincipleToAdd);
                }
                else
                {
                    var currentUserPermissions = await (from userPrinciples in _readWriteContext.UserPrinciples
                                                        where userPrinciples.UserAffiliationId == userAffiliationId
                                                              && userPrinciples.AccountId == message.AccountId
                                                        join principle in _readWriteContext.Principles on userPrinciples.PrincipleId equals principle.Id
                                                        where principle.PermissionType == PermissionAccessType.Regular
                                                        join permission in _readWriteContext.Permissions on principle.Id equals permission.PrincipleId
                                                        select permission).ToDictionaryAsync(x => new
                                                        {
                                                            x.Resource,
                                                            x.Action
                                                        });

                    foreach (var permission in message.Permissions)
                    {
                        if (currentUserPermissions.TryGetValue(new { permission.Resource, permission.Action }, out var currentPermission))
                        {
                            if (permission.Value.HasValue)
                            {
                                var newValue = permission.Value.Value ? PermissionValue.Granted : PermissionValue.Denied;
                                if (currentPermission.Value != newValue)
                                {
                                    var oldPermission = GeneratePermissionChangesObjectForActivityLogs(currentPermission);
                                    currentPermission.Value = newValue;
                                    var newPermission = GeneratePermissionChangesObjectForActivityLogs(currentPermission);
                                    permissionChanges.Add(newPermission, oldPermission);
                                }
                            }
                            else
                            {
                                var deletedPermission = GeneratePermissionChangesObjectForActivityLogs(currentPermission);
                                removedPermissions.Add(deletedPermission);

                                _readWriteContext.Set<Permission>().Remove(currentPermission);
                            }
                        }
                        else if (permission.Value.HasValue)
                        {
                            var newPermission = new Permission
                            {
                                AccountId = message.AccountId,
                                PrincipleId = userPrinciple.PrincipleId,
                                Resource = permission.Resource,
                                ResourceType = permission.ResourceType,
                                Action = permission.Action,
                                Value = permission.Value.Value ? PermissionValue.Granted : PermissionValue.Denied
                            };

                            var newPermissionLog = GeneratePermissionChangesObjectForActivityLogs(newPermission);
                            permissionChanges.Add(newPermissionLog, null);

                            _readWriteContext.Permissions.Add(newPermission);
                        }
                    }
                }

                try
                {
                    if (await _readWriteContext.SaveChangesAsync() > 0)
                    {
                        //Activity log is saved separately in this case,
                        //as we don't want to log updating permissions if no actual permissions were changed.
                        var activityLog = new ActivityLog
                        {
                            Id = SequentialGuid.Create(),
                            AccountId = message.AccountId,
                            EntityId = message.UserId,
                            EntityType = ActivityEntityType.User,
                            DomainId = message.UserId,
                            EntityName = $"{userAffiliation.FirstName} {userAffiliation.LastName}",
                            UserId = message.RequestUserId,
                            ActionId = (int)ActivityLogUserAction.UpdatePermissions
                        };

                        var activityLogDetails = new List<ActivityLogDetails>();

                        foreach (var permission in permissionChanges)
                        {
                            var newPermission = permission.Key;
                            var oldPermission = permission.Value;

                            var activityLogDetail = _activityLogHelper.GenerateActivityLogDetails(activityLog.Id, activityLog.AccountId, oldPermission, newPermission);
                            activityLogDetails.AddRange(activityLogDetail);
                        }

                        foreach (var permission in removedPermissions)
                        {
                            var activityLogDetail = _activityLogHelper.GenerateActivityLogDetails(activityLog.Id, activityLog.AccountId, permission, null);
                            activityLogDetails.AddRange(activityLogDetail);
                        }

                        _readWriteContext.ActivityLogs.Add(activityLog);
                        if (!activityLogDetails.IsNullOrEmpty())
                        {
                            _readWriteContext.ActivityLogDetails.AddRange(activityLogDetails);
                        }

                        await _readWriteContext.SaveChangesAsync();
                    }
                    return new Result(HttpStatusCode.NoContent);
                }
                catch (DbEntityValidationException e)
                {
                    Logger.Error(e, e.Format());
                }
                return new Result(HttpStatusCode.BadRequest);
            }

            public static PermissionChanges GeneratePermissionChangesObjectForActivityLogs(Permission permission)
            {
                var resource = int.Parse(permission.Resource);
                var resourceEnum = (ParentPermission)resource;
                var resourceValue = resourceEnum.GetDescription();
                var action = (PermissionActions)permission.Action;
                var actionValue = action.GetDescription();
                var value = permission.Value == PermissionValue.Granted ? "Granted" : "Denied";
                var permissionLog = new PermissionChanges()
                {
                    Permission = resourceValue + " : " + actionValue + " - " + value
                };

                return permissionLog;
            }
        }

        public class PermissionChanges
        {
            [ActivityLogDetails("Permission")]
            public string Permission { get; set; }
        }
    }
}
