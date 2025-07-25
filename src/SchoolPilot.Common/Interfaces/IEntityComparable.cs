

namespace SchoolPilot.Common.Interfaces
{
    public interface IEntityComparable
    {
        Guid Id { get; set; }
    }

    public class EntityComparableComparer<T> : IEqualityComparer<T>
        where T : IEntityComparable
    {
        public bool Equals(T x, T y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
