

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SchoolPilot.Common.Enums
{
    public enum Gender
    {
        [Description("Male")]
        Male = 1,

        [Description("Female")]
        Female = 2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EligibilityGender
    {
        [EnumMember(Value = "M")] Male,
        [EnumMember(Value = "F")] Female
    }
}
