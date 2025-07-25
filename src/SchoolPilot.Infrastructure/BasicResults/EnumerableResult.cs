using System.Collections;

namespace SchoolPilot.Infrastructure.BasicResults
{
    public class EnumerableResult<T> : IEnumerable<T>
    {
        private IEnumerable<T> enumerable;

        public EnumerableResult(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
