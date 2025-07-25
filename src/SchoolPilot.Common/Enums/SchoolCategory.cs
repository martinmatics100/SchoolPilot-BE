

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum SchoolCategory
    {
        [Description("Nursery")]
        Nusery = 1,

        [Description("Nursery And Primary")]
        NurseryAndPrimary,

        [Description("Nursery, Primary And Junior Secondary")]
        NurseryPrimaryAndJuniorSecondary,

        [Description("Nursery, Primary, Junior And Senior Secondary")]
        NurseryPrimaryAndBothSecondary,

        [Description("Junior And Senior Secondary")]
        Secondary,
    }
}
