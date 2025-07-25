

using SchoolPilot.Common.Extensions;
using SchoolPilot.Data.Attributes;
using SchoolPilot.Data.Entities.Logs;

namespace SchoolPilot.Infrastructure.Helpers
{
    public interface IActivityDomainTypesMapper
    {
        Dictionary<ActivityDomainType, List<ActivityEntityType>> GetActivityLogsGroupings();
    }

    public class ActivityDomainTypesMapper : IActivityDomainTypesMapper
    {
        private readonly Dictionary<ActivityDomainType, List<ActivityEntityType>> _activityLogsGroupings;

        public ActivityDomainTypesMapper()
        {
            _activityLogsGroupings = CreateActivityLogsGroupings();
        }

        public Dictionary<ActivityDomainType, List<ActivityEntityType>> GetActivityLogsGroupings()
        {
            return _activityLogsGroupings;
        }

        private static Dictionary<ActivityDomainType, List<ActivityEntityType>> CreateActivityLogsGroupings()
        {
            var activityLogsMapping = Enum.GetValues(typeof(ActivityDomainType))
                .Cast<ActivityDomainType>()
                .ToDictionary(k => k, v => new List<ActivityEntityType>());

            var customAttributes = EnumExtensions.GetMultiAttributes<ActivityEntityType, ActivityDomainTypeAttribute>();

            foreach (var customAttribute in customAttributes.Select(x => x.ActivityDomain).Distinct())
            {
                var activities = customAttributes
                    .Where(x => x.ActivityDomain == customAttribute)
                    .Select(x => x.ActivityEntity)
                    .ToList();

                if (activities.Any())
                {
                    activityLogsMapping[customAttribute].AddRange(activities);
                }
            }

            return activityLogsMapping;
        }
    }
}
