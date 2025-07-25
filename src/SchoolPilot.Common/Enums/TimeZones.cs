

using System.ComponentModel;
using SchoolPilot.Common.Attributes;

namespace SchoolPilot.Common.Enums
{
    public enum TimeZones
    {
        [Description("(GMT+10:00) Guam, Northern Mariana Islands")]
        [TimeZone("Pacific/Guam")]
        //[CustomDescription("(GMT+10:00) Guam, Northern Mariana Islands", "West Pacific Standard Time", ActualDescription = "Pacific/Guam")]
        WestPacificStandardTime = 1,
        [Description("(GMT-10:00) American Samoa")]
        [TimeZone("Pacific/Pago_Pago")]
        //[CustomDescription("(GMT-10:00) American Samoa", "Samoa Standard Time", ActualDescription = "Pacific/Pago_Pago")]
        SamoaStandardTime = 2,
        [Description("(GMT-10:00) Hawaii")]
        [TimeZone("Pacific/Honolulu")]
        //[CustomDescription("(GMT-10:00) Hawaii", "Hawaiian Standard Time", ActualDescription = "Pacific/Honolulu")]
        HawaiianStandardTime = 3,
        [Description("(GMT-09:00) Alaskan")]
        [TimeZone("America/Anchorage")]
        //[CustomDescription("(GMT-09:00) Alaskan", "Alaskan Standard Time", ActualDescription = "America/Anchorage")]
        AlaskanStandardTime = 4,
        [Description("(GMT-07:00) Pacific Time")]
        [TimeZone("America/Los_Angeles")]
        //[CustomDescription("(GMT-07:00) Pacific Time", "Pacific Standard Time", ActualDescription = "America/Los_Angeles")]
        PacificStandardTime = 5,
        [Description("(GMT-06:00) Mountain Time")]
        [TimeZone("America/Denver")]
        //[CustomDescription("(GMT-06:00) Mountain Time", "Mountain Standard Time", ActualDescription = "America/Denver")]
        MountainStandardTime = 6,
        // Arizona is the only state that officially does NOT observe Daylight Savings, making it an independent list item.
        [Description("(GMT-06:00) Arizona")]
        [TimeZone("America/Phoenix")]
        //[CustomDescription("(GMT-06:00) Arizona", "US Mountain Standard Time", ActualDescription = "America/Phoenix")]
        USMountainStandardTime = 7,
        [Description("(GMT-05:00) Central Time")]
        [TimeZone("America/Chicago")]
        //[CustomDescription("(GMT-05:00) Central Time", "Central Standard Time", ActualDescription = "America/Chicago")]
        CentralStandardTime = 8,
        [Description("(GMT-04:00) Eastern Time")]
        [TimeZone("America/New_York")]
        //[CustomDescription("(GMT-04:00) Eastern Time", "Eastern Standard Time", ActualDescription = "America/New_York")]
        EasternStandardTime = 9,
        [Description("(GMT-04:00) Puerto Rico, U.S. Virgin Islands")]
        [TimeZone("America/Puerto_Rico")]
        //[CustomDescription("(GMT-04:00) Puerto Rico, U.S. Virgin Islands", "Atlantic Standard Time", ActualDescription = "America/Puerto_Rico")]
        AtlanticStandardTime = 10
    }
}
