using System;
using System.Globalization;

namespace RedSeatServer.Extensions
{
    public static class DateExtensions
    {
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DayOfWeek? ToDayOfWeek(this string dayOfWeekString)
        {
            if (Enum.TryParse<DayOfWeek>(dayOfWeekString, out var dayOfWeek))
                return dayOfWeek;
            return null;
        }

        public static TimeSpan TimeOfDay(this string time) {
                DateTime dateTime = DateTime.ParseExact(time,
                                                    "h:mm tt", CultureInfo.InvariantCulture);
                return dateTime.TimeOfDay;
        }
        public static String TimeOfDayString(this TimeSpan time) {
            return time.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }
    }
}