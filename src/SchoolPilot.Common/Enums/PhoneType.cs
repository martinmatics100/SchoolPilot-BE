

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum PhoneType
    {
        [Description("Home")]
        Home = 1,

        [Description("Facility")]
        Facility = 2,

        [Description("Mobile")]
        Mobile = 3,

        [Description("Work")]
        Work = 4,

        [Description("Office")]
        Office = 5
    }
}
