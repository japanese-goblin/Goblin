using System;
using System.Globalization;

namespace Goblin.WebApp.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetWeekNumber(this DateTime date)
        {
            var ciCurr = CultureInfo.CurrentCulture;
            var weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek,
                                                        CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            return weekNum;
        }
        
        public static DateTime GetStartOfWeek(this DateTime dt)
        {
            var startOfWeek = DayOfWeek.Monday;
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}