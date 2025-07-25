

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum EmploymentType
    {
        [Description("Full Time")]
        FullTime = 1,
        [Description("Part Time")]
        PartTime = 2,
        [Description("Contract")]
        Contract = 3
    }
}
