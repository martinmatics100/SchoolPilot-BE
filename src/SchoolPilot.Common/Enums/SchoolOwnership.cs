

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum SchoolOwnership
    {
        [Description("Private School")]
        Private = 1,

        [Description("Military School")]
        Military,

        [Description("Missionary School")]
        Missionary,

        [Description("Federal Govt; School")]
        FederalGovt,

        [Description("State Govt; School")]
        StateGovt,

        [Description("Community School")]
        Community
    }
}
