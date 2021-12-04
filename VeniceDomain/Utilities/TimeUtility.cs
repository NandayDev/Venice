using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VeniceDomain.Enums;

namespace VeniceDomain.Utilities
{
    public static class TimeUtility
    {
        /// <summary>
        /// Returns the number of TimeComponent between the two dates
        /// </summary>
        public static int GetDifferenceBetweenDates(TimeComponent timeComponent, DateTime firstDate, DateTime secondDate)
        {
            return (int)((firstDate.Ticks - secondDate.Ticks) / timeComponent.GetTicks());
        }

        public static DateTime AddPeriodToDate(DateTime startDate, CandlePeriod candlePeriod) => startDate.AddSeconds(candlePeriod.ConvertToSeconds());

        public static long GetTicks(this TimeComponent timeComponent)
        {
            return timeComponent switch
            {
                TimeComponent.SECOND => TimeSpan.FromSeconds(1).Ticks,
                TimeComponent.MINUTE => TimeComponent.SECOND.GetTicks() * 60,
                TimeComponent.HOUR => TimeComponent.MINUTE.GetTicks() * 60,
                TimeComponent.DAY => TimeComponent.HOUR.GetTicks() * 24,
                TimeComponent.WEEK => TimeComponent.DAY.GetTicks() * 7,
                TimeComponent.YEAR => TimeComponent.DAY.GetTicks() * 365,
                _ => throw new ArgumentException(),
            };
        }

        public static bool AreDatesInTheSameWeek(DateTime date1, DateTime date2)
        {
            DateTime d1 = date1.Date.AddDays(-1 * GetDayOfWeekInteger(date1));
            DateTime d2 = date2.Date.AddDays(-1 * GetDayOfWeekInteger(date2));
            return d1 == d2;
        }

        private static int GetDayOfWeekInteger(DateTime date)
        {
            Calendar cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            int intDay = (int)cal.GetDayOfWeek(date);
            if (intDay == 0)
                intDay = 7;
            return intDay;
        }
    }

    public enum TimeComponent
    {
        SECOND,
        MINUTE,
        HOUR,
        DAY,
        WEEK,
        YEAR
    }
}
