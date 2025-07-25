

using SchoolPilot.Common.Enums;

namespace SchoolPilot.Common.Attributes
{
    public class AvailableActionsAttribute : Attribute
    {
        public List<PermissionActions> AvailableActions;

        public AvailableActionsAttribute(params PermissionActions[] actions)
        {
            AvailableActions = actions.ToList();
        }
    }
}
