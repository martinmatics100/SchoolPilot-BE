

using NodaTime;

namespace SchoolPilot.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime MinDate = new DateTime(1700, 1, 1);

        public static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 0);

        //Used 0 for the milliseconds due to a change in mysql 5.6 where the milliseconds are rounded.

        public static bool IsValid(this DateTime target)
        {
            return (target.Date >= MinDate.Date) && (target.Date < MaxDate.Date);
        }

        public static bool IsValid(this DateTime target, DateTime time)
        {
            return (target >= time) && (target < MaxDate);
        }

        public static bool IsWeekend(this DateTime target)
        {
            return target.DayOfWeek == DayOfWeek.Sunday || target.DayOfWeek == DayOfWeek.Saturday;
        }

        public static bool IsInPast(this DateTime target)
        {
            if (target.IsValid())
            {
                return DateTime.Today.Date > target.Date;
            }

            return false;
        }

        public static string ToHourMinTime(this DateTime target)
        {
            if (target.Hour > 0 || target.Minute > 0)
            {
                return target.ToString("H:mm");
            }

            return string.Empty;
        }

        public static string ToMySqlDate(this DateTime target)
        {
            if (target.IsValid())
            {
                return target.ToString("yyyy-MM-dd");
            }

            return string.Empty;
        }

        public static string ToJavascriptFormat(this DateTime dateTime)
        {
            return dateTime.ToString("MMM d, yyyy");
        }

        public static string ToJavascriptUTC(this DateTime dateTime)
        {
            return "Date.UTC(" + dateTime.Year + "," + dateTime.AddMonths(-1).Month + "," + dateTime.Day + "," + dateTime.Hour + "," + dateTime.Minute + ")";
        }

        public static bool IsBetween(this DateTime dateTime, DateTime startDate, DateTime endDate)
        {
            if (dateTime.IsValid())
            {
                return dateTime.Ticks >= startDate.Ticks && dateTime.Ticks <= endDate.Ticks;
            }

            return false;
        }

        public static DateTime ToAvailabilityDate(this DateTime dateTime)
        {
            return new DateTime(1, 1, (int)dateTime.DayOfWeek + 1, dateTime.Hour, dateTime.Minute, 0);
        }

        public static string ToTinyTimeFormat(this DateTime time)
        {
            return time.ToString("h:mm tt");
        }

        public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks / 2) / d.Ticks) * d.Ticks, dt.Kind);
        }

        public static DateTime CombineDateAndTime(this DateTime date, DateTime? time)
        {
            return time == null ? date.Date : new DateTime(date.Year, date.Month, date.Day, time.Value.Hour, time.Value.Minute, time.Value.Second);
        }

        public static int DiffDays(this DateTime date, DateTime end)
        {
            return (date - end).Duration().Days;
        }

        public static DateTime GetEndOfWeek(DayOfWeek lastDayOfWeek, DateTime date)
        {
            if (date.DayOfWeek >= lastDayOfWeek)
                return date;

            while (date.DayOfWeek < lastDayOfWeek)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime EndOfPreviousMonth(this DateTime date)
        {
            return date.AddDays(-1 * date.Day);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static bool IsStartOfMonth(this DateTime date)
        {
            return date.Day == 1;
        }

        public static bool IsEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day) == new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var num = dt.DayOfWeek - startOfWeek;
            if (num < 0)
                num += 7;

            return dt.AddDays((double)(-1 * num)).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek endOfWeek)
        {
            var num = endOfWeek - dt.DayOfWeek;
            if (num < 0)
                num += 7;

            return dt.AddDays((num)).Date;
        }

        public static DateTime BiWeeklyEndDate(this DateTime dt, DayOfWeek endOfWeek)
        {
            var num = endOfWeek - dt.DayOfWeek;
            if (num < 0)
                num += 14;
            else
                num += 7;

            return dt.AddDays(num).Date;
        }

        public static DateTime GetToday(this DateTimeZone timeZone)
        {
            return new ZonedDateTime(Instant.FromDateTimeUtc(DateTime.UtcNow), timeZone).ToDateTimeUnspecified().Date;
        }

        public static DateTime GetNextBusinessDay(this DateTimeZone timeZone)
        {
            var today = timeZone.GetToday();
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return today.AddDays(3);
                case DayOfWeek.Saturday:
                    return today.AddDays(2);
                default:
                    return today.AddDays(1);
            }
        }

        public static DateTime GetNow(this DateTimeZone timeZone)
        {
            return new ZonedDateTime(Instant.FromDateTimeUtc(DateTime.UtcNow), timeZone).ToDateTimeUnspecified();
        }

        public static DateTime ConvertLocalTimeToUtc(this DateTime dateTime, DateTimeZone timeZone)
        {
            if (dateTime == MaxDate || dateTime == DateTime.MaxValue)
            {
                return MaxDate;
            }

            var localDateTime = LocalDateTime.FromDateTime(dateTime);
            var zonedDateTime = localDateTime.InZoneStrictly(timeZone);
            return zonedDateTime.ToDateTimeUtc();
        }

        public static DateTimeOffset ToOffset(this DateTime dateTime, DateTimeZone timeZone)
        {
            if (dateTime == MaxDate || dateTime == DateTime.MaxValue)
            {
                return MaxDate;
            }

            var localDateTime = LocalDateTime.FromDateTime(dateTime);
            var zonedDateTime = localDateTime.InZoneStrictly(timeZone);
            return zonedDateTime.ToDateTimeOffset();
        }

        public static DateTimeOffset ToOffsetFromUtc(this DateTime dateTime, DateTimeZone timeZone)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime must be of kind UTC.", nameof(dateTime));
            }

            if (dateTime == MaxDate || dateTime == DateTime.MaxValue)
            {
                return MaxDate;
            }

            return Instant.FromDateTimeUtc(dateTime).InZone(timeZone).ToDateTimeOffset();
        }

        public static ZonedDateTime ToZonedDateTime(this DateTime dateTime, DateTimeZone timeZone)
        {
            var localDateTime = LocalDateTime.FromDateTime(dateTime);
            return localDateTime.InZoneStrictly(timeZone);
        }

        public static ZonedDateTime ToZonedDateTimeFromUtc(this DateTime dateTime, DateTimeZone timeZone)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime must be of kind UTC.", nameof(dateTime));
            }

            return Instant.FromDateTimeUtc(dateTime).InZone(timeZone);
        }

        public static int GetDayOfWeekFlag(this DateTime date)
        {
            return (int)Math.Pow(2, (int)date.DayOfWeek);
        }

        public static DateTime? ConvertUtcToLocalTime(this DateTime? utcTime, DateTimeZone timeZone)
        {
            if (utcTime.HasValue)
            {
                if (utcTime == MaxDate || utcTime == DateTime.MaxValue)
                {
                    return MaxDate;
                }

                return new ZonedDateTime(Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcTime.Value, DateTimeKind.Utc)), timeZone).ToDateTimeUnspecified();
            }

            return null;
        }

        public static DateTime? ConvertUtcToLocalTime(this DateTime? utcTime, string timeZone)
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone);
            if (utcTime.HasValue && !timeZone.IsNullOrEmpty())
            {
                if (utcTime == MaxDate || utcTime == DateTime.MaxValue)
                {
                    return MaxDate;
                }

                return new ZonedDateTime(Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcTime.Value, DateTimeKind.Utc)), dateTimeZone).ToDateTimeUnspecified();
            }

            return null;
        }

        public static DateTime ConvertUtcToLocalTime(this DateTime utcTime, DateTimeZone timeZone)
        {
            if (utcTime == MaxDate || utcTime == DateTime.MaxValue)
            {
                return MaxDate;
            }

            return new ZonedDateTime(Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcTime, DateTimeKind.Utc)), timeZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncate DateTime</returns>
        public static DateTime TruncateTo(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        public static DateTime TimeAsDate(this DateTime target)
        {
            return new DateTime(1, 1, 1, target.Hour, target.Minute, 0);
        }

        /// <summary>
        /// Gets the federal fiscal year which is from Oct/01/XXXX – 09/30/XXXX
        /// </summary>
        /// <param name="year">The year of the end date. 2019 year refers to Oct/01/2018 – 09/30/2019.</param>
        /// <returns></returns>
        public static (DateTime StartDate, DateTime EndDate) GetFederalFiscalYear(int year)
        {
            return (new DateTime(year - 1, 10, 1), new DateTime(year, 9, 30));
        }

        public static DateTime ToEndOfDay(this DateTime target)
        {
            return new DateTime(target.Year, target.Month, target.Day, 23, 59, 59);
        }

        public static DateTime Max(params DateTime[] dates) => dates.Max();

        public static DateTime Min(params DateTime[] dates) => dates.Min();

        public static DateTime MaxDateTime(DateTime date1, DateTime date2)
        {
            return new DateTime(Math.Max(date1.Ticks, date2.Ticks));
        }

        public static DateTime MinDateTime(DateTime date1, DateTime date2)
        {
            return new DateTime(Math.Min(date1.Ticks, date2.Ticks));
        }

        public static DateTime FromMinutes(int minutes)
        {
            return DateTime.MinValue.AddMinutes(minutes);
        }

        /// <summary>
        /// Gets minutes worth of daylight savings occurs
        /// </summary>
        /// <returns></returns>
        public static int GetDaylightSavingTransitions(DateTimeZone timeZone, DateTime startDate, DateTime endDate)
        {
            var start = new LocalDateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, startDate.Minute).InZoneLeniently(timeZone).ToInstant();
            var end = new LocalDateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute).InZoneLeniently(timeZone).ToInstant();
            var intervals = timeZone.GetZoneIntervals(start, end).ToList();
            var savings = 0;
            if (intervals.Count == 2)
            {
                savings = (intervals[0].Savings.Seconds - intervals[1].Savings.Seconds) / 60;
            }

            return savings;
        }

        /// <summary>
        /// Compares date with MaxDate and returns null if it matches
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetMaxDate(this DateTime date)
        {
            if (date.Equals(MaxDate))
            {
                return null;
            }

            return date;
        }

        public static int CalculateTimeFromAdmissionToEstablishAdvanceDirective(DateTime? dateAdvanceDirectiveIsMarkedYes, DateTime admissionDate)
        {
            if (dateAdvanceDirectiveIsMarkedYes != null)
                return (int)(dateAdvanceDirectiveIsMarkedYes.Value - admissionDate).TotalDays;

            return 0;
        }

        public static double ToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static int GetAge(this DateTime date)
        {
            var age = DateTime.Today.Year - date.Year;
            if (date.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
