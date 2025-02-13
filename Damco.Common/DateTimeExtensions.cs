using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert a date/time in a specific timezone to the equivalent UTC date/time.
        /// </summary>
        /// <param name="source">The source date/time.</param>
        /// <param name="sourceTimeZone">The timezone of source. If null, Utc is assumed.</param>
        /// <returns>source calculated to UTC, if source.Kind is not already Utc.</returns>
        public static DateTime ToUtc(this DateTime source, TimeZoneInfo sourceTimeZone)
        {
            if (source.Kind == DateTimeKind.Utc)
                return source;
            else if (sourceTimeZone == null)
                return DateTime.SpecifyKind(source, DateTimeKind.Utc);
            else if (source.Kind == DateTimeKind.Local) //Don't let it go wrong - ConvertTimeToUtc demands the source timezone to be the machine timezone if the kind is Local
                return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(source, DateTimeKind.Unspecified), sourceTimeZone);
            else
                return TimeZoneInfo.ConvertTimeToUtc(source, sourceTimeZone);
        }

        public static DateTime FromUtc(this DateTime utc, TimeZoneInfo targetTimeZone)
        {
            if (utc.Kind == DateTimeKind.Local)
                return utc;
            else if (targetTimeZone == null)
                return DateTime.SpecifyKind(utc, DateTimeKind.Local);
            else
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(utc, targetTimeZone), DateTimeKind.Local);
        }

        public static DateTime ToStartOfLocalDay(this DateTime utc, TimeZoneInfo localTimeZone)
        {
            return utc.FromUtc(localTimeZone).Date.ToUtc(localTimeZone);
        }

        public static DateTime LocalNow(this TimeZoneInfo timeZone)
        {
            return DateTime.UtcNow.FromUtc(timeZone);
        }
    }
}
