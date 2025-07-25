

using SchoolPilot.Data.Entities.Logs;

namespace SchoolPilot.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ActivityDomainTypeAttribute : Attribute
    {
        public object ActivityId;

        public ActivityDomainType ActivityDomain;

        public ActivityEntityType ActivityEntity;

        public override object TypeId => ActivityId;

        public ActivityDomainTypeAttribute(ActivityEntityType activityEntity, ActivityDomainType activityDomain)
        {
            ActivityEntity = activityEntity;
            ActivityDomain = activityDomain;
            ActivityId = Guid.NewGuid();
        }
    }
}
