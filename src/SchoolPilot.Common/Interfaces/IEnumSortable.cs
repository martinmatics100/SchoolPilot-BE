

namespace SchoolPilot.Common.Interfaces
{
    /// <summary>
    /// This is used to have the inheriting model get an enum expression tree inserted into its query select,
    ///  when used with QueryableExtensions.AddSortedRequestOrdering.
    /// Any model that uses this should also be using the Sortable attribute to define the BindOrder of its properties.
    /// </summary>
    public interface IEnumSortable<T> : IQueryBinding
    {
        T EnumSortingValue { get; set; }
    }

    public interface IAlphanumericAggregateSortable<T> : IQueryBinding
    {
        T AlphaNumAggregate { get; set; }
    }

    public interface IAlphanumericSortable<T> : IQueryBinding
    {
        T AlphaNumConditionalOrder { get; set; }
    }

    public interface IAlphanumericLengthSortable<T> : IQueryBinding
    {
        T AlphaNumLength { get; set; }
    }

    public interface IQueryBinding
    {
    }
}
