
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequireNotEmptyAttribute : ValidationAttribute

    {
        public RequireNotEmptyAttribute()
            : base(@"{0} is required.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                switch (value)
                {
                    case Array array:
                        return array.Length > 0;
                    case IList list:
                        return list.Count > 0;
                    case ICollection collection:
                        return collection.Count > 0;
                    case HashSet<Guid> set:
                        return set.Count > 0;
                    case string str:
                        return str != string.Empty;
                }
            }

            return false;
        }
    }
}
