

using System.ComponentModel;

namespace SchoolPilot.Common.Enums
{
    public enum SubjectCategory
    {
        [Description("Core Primary")]
        IsPrimary,

        [Description("Core Junior Secondary")]
        IsJuniorSecondary,

        [Description("Core Senior Secondary")]
        IsSeniorSecondaryCore,

        [Description("Core Science Senior Secondary")]
        IsSeniorSecondaryScienceStreamCore,

        [Description("Optional Science Senior Secondary")]
        IsSeniorSecondaryScienceStreamOptional,

        [Description("Core Arts Senior Secondary")]
        IsSeniorSecondaryArtsStreamCore,

        [Description("Optional Arts Senior Secondary")]
        IsSeniorSecondaryArtsStreamOptional,

        [Description("Core Commercial Senior Secondary")]
        IsSeniorSecondaryCommercialStreamCore,

        [Description("Optional Commercial Senior Secondary")]
        IsSeniorSecondaryCommercialStreamOptional,

    }
}
