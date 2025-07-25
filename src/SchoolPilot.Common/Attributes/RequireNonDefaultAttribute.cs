
using System.ComponentModel.DataAnnotations;

namespace SchoolPilot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequireNonDefaultAttribute : ValidationAttribute
    {
        public RequireNonDefaultAttribute() : base(@"{0} is required")
        {

        }

        public RequireNonDefaultAttribute(string errorMessage) : base(errorMessage)
        {

        }

        public override bool IsValid(object value)
        {
            return value != null && !Equals(value, Activator.CreateInstance(value.GetType()));
        }
    }
}
