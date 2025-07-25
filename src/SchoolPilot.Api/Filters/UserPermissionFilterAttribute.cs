using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;
using SchoolPilot.Api.Controllers;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Entities.Users;
using SchoolPilot.Infrastructure.Queries.Users;
using StackExchange.Redis;
using System.Net;
using System.Threading.Tasks;

namespace SchoolPilot.Api.Filters
{
    public class UserPermissionFilterAttribute : ActionFilterAttribute
    {
        private readonly ResourceType _resourceType;
        private readonly HashSet<string> _resource;
        private readonly PermissionActions[] _actions;
        private readonly RedisValue[] _permissionKeys;
        private static readonly int DeniedPermissionValue = (int)PermissionValue.Denied;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UserPermissionFilterAttribute(ResourceType resourceType, ParentPermission resource, params PermissionActions[] actions)
        {
            _resourceType = resourceType;
            _resource = new HashSet<string> { ((int)resource).ToString() };
            _actions = actions;
            _permissionKeys = CreatePermissionKeyArray(resourceType, _resource, actions);
        }

        public UserPermissionFilterAttribute(ResourceType resourceType, ParentPermission[] resource, params PermissionActions[] actions)
        {
            _resourceType = resourceType;
            _resource = resource.Select(x => ((int)x).ToString()).ToHashSet();
            _actions = actions;
            _permissionKeys = CreatePermissionKeyArray(resourceType, _resource, actions);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is IBaseApiController controller)
            {
                var success = await HandlePermissionValidationAsync(controller);
                if (!success)
                {
                    context.Result = new ObjectResult("Client does not have the permissions to access this endpoint.")
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }
            }

            await next();
        }

        const string HaspermissionsHashFieldName = "hasPermissions";
        const string HasPermissionComparisonValue = "true";

        private async Task<bool> HandlePermissionValidationAsync(IBaseApiController controller)
        {
            var isRedisDown = false;
            var userAffiliationId = await controller.GetSingleUserAffiliationId();
            var hashKey = "userPermissions:" + userAffiliationId;

            // Updated to use IRedisDatabase directly
            var redisDatabase = controller.RedisCache.GetDefaultDatabase().Database;

            try
            {
                var hashValues = await redisDatabase.HashGetAsync(hashKey, _permissionKeys);
                //Check if the flag that states permissions have been stored is equal to "true"
                if (hashValues[0].HasValue)
                {
                    var hasPermission = false;
                    for (var i = 1; i < hashValues.Length && !hasPermission; i++)
                    {
                        //Check if the permission that is stored in redis is marked as granted (0) and compare that is not equal to denied (1).
                        // also need to check that the value is not null, otherwise the default int value is 0 (granted)
                        if (hashValues[i].TryParse(out int val))
                        {
                            hasPermission = val != DeniedPermissionValue;
                        }
                    }

                    return hasPermission;
                }
            }
            catch (RedisConnectionException exception)
            {
                isRedisDown = true;
                Logger.Error(exception);
            }
            catch (RedisTimeoutException exception)
            {
                isRedisDown = true;
                Logger.Error(exception);
            }

            //Since the flag was not found then we need to pull and store the permissions for later.
            return await UpdatePermissionCache(controller, userAffiliationId, hashKey, isRedisDown, redisDatabase);
        }

        private async Task<bool> UpdatePermissionCache(IBaseApiController controller, Guid userAffiliationId, string hashKey, bool isRedisDown, IDatabase redisDatabase)
        {
            var permissionContainer = await controller.Mediator.Send(new GetPermissions.Query
            {
                UserAffiliationId = userAffiliationId,
                AccountId = controller.AccountId.GetValueOrDefault()
            });

            var result = false;
            var permissionsToCache = new HashEntry[permissionContainer.Permissions.Count + 1];
            permissionsToCache[0] = new HashEntry(HaspermissionsHashFieldName, HasPermissionComparisonValue);
            for (var i = 0; i < permissionContainer.Permissions.Count; i++)
            {
                var permission = permissionContainer.Permissions[i];

                permissionsToCache[i + 1] = new HashEntry(
                    CreatePermissionKey(permission.ResourceType, permission.Resource, permission.Action),
                    ((int)permission.Value).ToString()
                );

                if (permission.ResourceType != _resourceType || !_resource.Contains(permission.Resource))
                {
                    continue;
                }

                for (var x = 0; x < _actions.Length && !result; x++)
                {
                    if (permission.Action == _actions[x] &&
                        permission.Value == PermissionValue.Granted)
                    {
                        result = true;
                    }
                }
            }

            try
            {
                if (!isRedisDown)
                {
                    await redisDatabase.HashSetAsync(hashKey, permissionsToCache);
                }
            }
            catch (RedisConnectionException exception)
            {
                Logger.Error(exception);
            }
            catch (RedisTimeoutException exception)
            {
                Logger.Error(exception);
            }
            return result;
        }

        private static RedisValue[] CreatePermissionKeyArray(ResourceType resourceType, HashSet<string> resources, PermissionActions[] actions)
        {
            var permissionKeys = new List<RedisValue>();
            foreach (var resource in resources)
            {
                permissionKeys.AddRange(actions
                    .Select(action => CreatePermissionKey(resourceType, resource, action))
                    .Select(x => (RedisValue)x));
            }

            // This is to prevent having to add this value into the array when attempting to fetch values
            return new RedisValue[]
            {
                HaspermissionsHashFieldName
            }.Concat(permissionKeys).ToArray();
        }

        private static string CreatePermissionKey(ResourceType resourceType, string resource, PermissionActions action)
        {
            return $"type:{(int)resourceType}:resource:{resource}:action:{(int)action}";
        }
    }
}