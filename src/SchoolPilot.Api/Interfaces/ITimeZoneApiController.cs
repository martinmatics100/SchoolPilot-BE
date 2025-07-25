
using NodaTime;

namespace SchoolPilot.Api.Interfaces
{
    public interface ITimeZoneApiController
    {
        string TimeZoneString { get; set; }

        DateTimeZone TimeZone { get; }
    }
}
