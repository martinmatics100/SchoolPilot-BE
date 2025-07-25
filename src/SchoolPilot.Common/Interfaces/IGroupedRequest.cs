

namespace SchoolPilot.Common.Interfaces
{
    public interface IGroupedRequest
    {
        string GroupBy { get; set; }

        string GroupOrder { get; set; }
    }
}
