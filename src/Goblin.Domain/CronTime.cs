using System;

namespace Goblin.Domain
{
    public class CronTime
    {
        private const string AnyChar = "*";
        public string Minute { get; private set; }
        public string Hour { get; private set; }
        public string DayOfMonth { get; private set; }
        public string Month { get; private set; }
        public string DayOfWeek { get; private set; }

        protected CronTime()
        {
        }

        public CronTime(string minute = AnyChar, string hour = AnyChar, string dayOfMonth = AnyChar, string month = AnyChar,
                        string dayOfWeek = AnyChar)
        {
            SetMinute(minute);
            SetHour(hour);
            SetDayOfMonth(dayOfMonth);
            SetMonth(month);
            SetDayOfWeek(dayOfWeek);
        }

        public void SetMinute(string minute)
        {
            if(!minute.Equals(AnyChar))
            {
                if(!int.TryParse(minute, out var intMinute))
                {
                    throw new ArgumentException("Параметр не является числом", nameof(minute));
                }

                if(intMinute < 0 || intMinute > 59)
                {
                    throw new ArgumentOutOfRangeException(nameof(minute), minute,
                                                          "Параметр должен быть в пределах от 0 до 60");
                }
            }

            Minute = minute;
        }

        public void SetHour(string hour)
        {
            if(!hour.Equals(AnyChar))
            {
                if(!int.TryParse(hour, out var intHour))
                {
                    throw new ArgumentException("Параметр не является числом", nameof(hour));
                }

                if(intHour < 0 || intHour > 23)
                {
                    throw new ArgumentOutOfRangeException(nameof(hour), hour,
                                                          "Параметр должен быть в пределах от 0 до 23");
                }
            }

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