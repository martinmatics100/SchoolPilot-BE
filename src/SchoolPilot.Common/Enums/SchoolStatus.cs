

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum SchoolStatus
    {
        [Description("Active")]
        Active = 1,
        [Description("Suspended")]
        Suspended,
        [Description("Not-Active")]
        Inactive,
        [Description("Closed")]
        Closed
    }
}
