

using NodaTime;

namespace SchoolPilot.Common.Attributes
{
    public class TimeZoneAttribute : Attribute
    {
        private readonly string _timeZone;

        public TimeZoneAttribute(string timeZone)
        {
            _timeZone = timeZone;
        }

        public DateTimeZone GetTimeZone()
        {
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(_timeZone);
        }
    }
}
