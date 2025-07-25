
using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum Country
    {
        [Description("United States|US|+1|🇺🇸")]
        UnitedStates = 1,

        [Description("United Kingdom|GB|+44|🇬🇧")]
        UnitedKingdom = 2,

        [Description("Nigeria|NG|+234|🇳🇬")]
        Nigeria = 3,

        [Description("Germany|DE|+49|🇩🇪")]
        Germany = 4,
    }
}
