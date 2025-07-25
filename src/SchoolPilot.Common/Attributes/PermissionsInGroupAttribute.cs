

using SchoolPilot.Common.Enums;

namespace SchoolPilot.Common.Attributes
{
    public class PermissionsInGroupAttribute : Attribute
    {
        public List<ParentPermission> PermissionsInGroup;

        public PermissionsInGroupAttribute(params ParentPermission[] permissionsInGroup)
        {
            PermissionsInGroup = permissionsInGroup.ToList();
        }
    }
}
