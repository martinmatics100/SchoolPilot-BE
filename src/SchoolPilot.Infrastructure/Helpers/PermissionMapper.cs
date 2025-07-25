

using SchoolPilot.Common.Attributes;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Entities.Users;
using System.Reflection;

namespace SchoolPilot.Infrastructure.Helpers
{
    public class PermissionModel
    {
        public string Resource { get; set; }

        public ResourceType ResourceType { get; set; }

        public PermissionActions Action { get; set; }

        public bool? Value { get; set; }
    }

    public class PermissionGroupModel
    {
        public string? GroupName { get; set; }

        public List<int>? ParentPermissions { get; set; }

        public List<int>? AvailableActions { get; set; }

        public Dictionary<int, List<int>>? FeatureAvailableActions { get; set; }
    }

    public interface IPermissionMapper
    {
        List<PermissionModel> GetPermissionList();

        List<PermissionGroupModel> GetPermissionGroups();
        // 🔁 Changed from Dictionary<PermissionDependencyKey, List<PermissionActions>>
        Dictionary<string, List<PermissionActions>> GetPermissionDependencies();

    }

    public class PermissionMapper : IPermissionMapper
    {
        private readonly List<PermissionModel> _permissions;
        private readonly List<PermissionGroupModel> _permissionGroups;
        // ✅ Use the new class as dictionary key
        private readonly Dictionary<PermissionDependencyKey, List<PermissionActions>> _permissionDependencies;

        public PermissionMapper()
        {
            _permissions = CreatePermissionList();
            _permissionGroups = CreatePermissionGroups();
            _permissionDependencies = CreatePermissionDependencies();
        }

        public List<PermissionModel> GetPermissionList()
        {
            return _permissions;

        }

        public List<PermissionGroupModel> GetPermissionGroups()
        {
            return _permissionGroups;
        }

        public Dictionary<string, List<PermissionActions>> GetPermissionDependencies()
        {
            var stringKeyedDependencies = new Dictionary<string, List<PermissionActions>>();

            foreach (var kvp in _permissionDependencies)
            {
                // 🔁 Convert custom key object to string
                var keyString = $"{kvp.Key.ParentPermission}_{kvp.Key.PermissionAction}";
                stringKeyedDependencies[keyString] = kvp.Value;
            }

            return stringKeyedDependencies;
        }

        private List<PermissionModel> CreatePermissionList()
        {
            var permissionMapping = new List<PermissionModel>();
            var enumValues = Enum.GetValues(typeof(ParentPermission)).Cast<ParentPermission>();
            foreach (var value in enumValues)
            {
                var availableActionsAttribute = typeof(ParentPermission)?.GetField(value.ToString())?.GetCustomAttribute<AvailableActionsAttribute>();
                if (availableActionsAttribute != null)
                {
                    permissionMapping.AddRange(availableActionsAttribute.AvailableActions.Select(availableAction => new PermissionModel
                    {
                        Action = availableAction,
                        Resource = ((int)value).ToString(),
                        ResourceType = ResourceType.Categorized,
                        Value = null
                    }));
                }
            }

            return permissionMapping;
        }

        private List<PermissionGroupModel> CreatePermissionGroups()
        {
            var permissionGroups = new List<PermissionGroupModel>();
            var enumValues = Enum.GetValues(typeof(PermissionGroup)).Cast<PermissionGroup>();
            var parentPermissionValues = Enum.GetValues(typeof(ParentPermission)).Cast<ParentPermission>();

            foreach (var value in enumValues)
            {
                var permissionsInGroupAttribute = typeof(PermissionGroup)?
                    .GetField(value.ToString())?
                    .GetCustomAttribute<PermissionsInGroupAttribute>();

                var availableActionsAttribute = typeof(PermissionGroup)?
                    .GetField(value.ToString())?
                    .GetCustomAttribute<AvailableActionsAttribute>();

                if (permissionsInGroupAttribute != null && availableActionsAttribute != null)
                {
                    var groupModel = new PermissionGroupModel
                    {
                        GroupName = value.ToString(),
                        ParentPermissions = permissionsInGroupAttribute.PermissionsInGroup
                            .Select(x => (int)x).ToList(),
                        AvailableActions = availableActionsAttribute.AvailableActions
                            .Select(x => (int)x).ToList(),
                        FeatureAvailableActions = new Dictionary<int, List<int>>()
                    };

                    // Add available actions for each feature in this group
                    foreach (var permission in permissionsInGroupAttribute.PermissionsInGroup)
                    {
                        var permissionField = typeof(ParentPermission).GetField(permission.ToString());
                        var featureActionsAttribute = permissionField?
                            .GetCustomAttribute<AvailableActionsAttribute>();

                        if (featureActionsAttribute != null)
                        {
                            groupModel.FeatureAvailableActions[(int)permission] =
                                featureActionsAttribute.AvailableActions
                                    .Select(x => (int)x).ToList();
                        }
                    }

                    permissionGroups.Add(groupModel);
                }
            }

            return permissionGroups;
        }

        // ✅ Changed to use custom key class instead of ValueTuple
        private Dictionary<PermissionDependencyKey, List<PermissionActions>> CreatePermissionDependencies()
        {
            var permissionDependencyMapping = new Dictionary<PermissionDependencyKey, List<PermissionActions>>();
            var enumValues = Enum.GetValues(typeof(ParentPermission)).Cast<ParentPermission>();

            foreach (var value in enumValues)
            {
                var dependencies = typeof(ParentPermission)
                    ?.GetField(value.ToString())
                    ?.GetCustomAttributes<PermissionDependencyAttribute>();

                foreach (var dependency in dependencies)
                {
                    var key = new PermissionDependencyKey
                    {
                        ParentPermission = (int)value,
                        PermissionAction = (int)dependency.Key
                    };

                    permissionDependencyMapping[key] = dependency.Dependencies;
                }
            }

            return permissionDependencyMapping;
        }
    }
}
