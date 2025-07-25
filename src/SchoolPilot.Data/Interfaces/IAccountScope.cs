

using SchoolPilot.Common.Attributes;

namespace SchoolPilot.Data.Interfaces
{
    public interface IAccountScope
    {
        // Unique Id for each school
        [RequireNonDefault]
        Guid AccountId { get; set; }
    }
}
