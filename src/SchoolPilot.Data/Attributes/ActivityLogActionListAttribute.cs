

namespace SchoolPilot.Data.Attributes
{
    public class ActivityLogActionListAttribute : Attribute
    {
        public readonly Type ActionEnum;

        public ActivityLogActionListAttribute(Type actionEnum)
        {
            ActionEnum = actionEnum;
        }
    }
}
