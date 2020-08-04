using System;

namespace Goblin.Domain
{
    public class CronTime
    {
        public string Minute { get; private set; }
        public string Hour { get; private set; }
        public string DayOfMonth { get; private set; }
        public string Month { get; private set; }
        public string DayOfWeek { get; private set; }

        protected CronTime()
        {
        }

        public CronTime(string minute = "*", string hour = "*", string dayOfMonth = "*", string month = "*", string dayOfWeek = "*")
        {
            SetMinute(minute);
            SetHour(hour);
            SetDayOfMonth(dayOfMonth);
            SetMonth(month);
            SetDayOfWeek(dayOfWeek);
        }

        public void SetMinute(int minute)
        {
            if(minute < 0 || minute > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), minute,
                                                      "Параметр должен быть в пределах от 0 до 60");
            }

            SetMinute(minute.ToString());
        }

        public void SetMinute(string minute)
        {
            Minute = minute;
        }

        public void SetHour(int hour)
        {
            if(hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), hour,
                                                      "Параметр должен быть в пределах от 0 до 23");
            }

            SetHour(hour.ToString());
        }

        public void SetHour(string hour)
        {
            Hour = hour;
        }

        public void SetDayOfMonth(int day)
        {
            if(day < 1 || day > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(day), day,
                                                      "Параметр должен быть в пределах от 1 до 31");
            }

            SetDayOfMonth(day.ToString());
        }

        public void SetDayOfMonth(string day)
        {
            DayOfMonth = day;
        }

        public void SetMonth(int month)
        {
            if(month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), month,
                                                      "Параметр должен быть в пределах от 1 до 12");
            }

            SetMonth(month.ToString());
        }

        public void SetMonth(string month)
        {
            Month = month;
        }

        public void SetDayOfWeek(int dayOfWeek)
        {
            if(dayOfWeek < 0 || dayOfWeek > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek,
                                                      "Параметр должен быть в пределах от 1 до 6");
            }

            SetDayOfWeek(dayOfWeek.ToString());
        }

        public void SetDayOfWeek(string dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
        }

        public override string ToString()
        {
            return $"{Minute} {Hour} {DayOfMonth} {Month} {DayOfWeek}";
        }
    }
}