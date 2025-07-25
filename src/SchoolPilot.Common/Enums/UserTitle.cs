

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum UserTitle
    {
        CEO = 1,
        COO = 2,
        CFO = 3,
        Administrator = 4,
        [Description("Alternate Administrator")]
        AlternateAdministrator = 5,
    }
}
