

namespace SchoolPilot.Common.Interfaces
{
    public interface IMultiSortedRequest
    {
        string[] SortBy { get; set; }

        string Order { get; set; }
    }
}
