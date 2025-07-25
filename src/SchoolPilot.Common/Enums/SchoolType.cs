

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum SchoolType
    {
        [Description("Day School")]
        Day = 1,
        [Description("Boarding School")]
        Border,
        [Description("Day & Boarding School")]
        Both
    }
}
