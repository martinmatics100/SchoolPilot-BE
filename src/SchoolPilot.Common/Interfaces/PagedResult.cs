

namespace SchoolPilot.Common.Interfaces
{
    public class PagedResult<TItemType>
    {
        /// <summary>
        /// Total count of items in paginated query.
        /// </summary>
        public int ItemCount { get; set; }

        public int PageLength { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public IList<TItemType> Items { get; set; }
    }

    public interface IPagedRequest
    {
        int Page { get; set; }
        int PageLength { get; set; }
    }
}
