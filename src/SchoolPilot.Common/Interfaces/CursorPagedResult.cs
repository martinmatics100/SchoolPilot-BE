

namespace SchoolPilot.Common.Interfaces
{
    public class CursorPagedResult<TItemType, TCursorType>
    {
        public int PageLength { get; set; }

        public TCursorType NextCursor { get; set; }

        public IList<TItemType> Items { get; set; }

        public bool IsAtEnd { get; set; }
    }

    public interface ICursorPagedRequest<TCursorType> where TCursorType : struct
    {
        TCursorType? Cursor { get; set; }

        int PageLength { get; set; }
    }
}
