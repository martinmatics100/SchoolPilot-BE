

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum UserStatus
    {
        [Description("Pending Invite")]
        PendingInvite = 0,
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 2,
    }
}
