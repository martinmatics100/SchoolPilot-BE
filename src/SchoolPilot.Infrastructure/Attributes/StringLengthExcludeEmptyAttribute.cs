

using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Infrastructure.Attributes
{
    public class StringLengthExcludeEmptyAttribute : StringLengthAttribute
    {
        public StringLengthExcludeEmptyAttribute(int maximumLength) : base(maximumLength)
        {
        }

        public override bool IsValid(object value)
        {
            int length = ((string)value)?.Length ?? 0;
            return value == null || value.Equals(string.Empty) || (length >= this.MinimumLength && length <= this.MaximumLength);
        }
    }
}
