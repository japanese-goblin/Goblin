using System;

namespace Narfu.Models
{
    public class Lesson : IEquatable<Lesson>
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string StartEndTime { get; set; }
        public byte Number { get; set; }
        public string Address { get; set; }
        public string Auditory { get; set; }
        public string Teacher { get; set; }
        public string Groups { get; set; }

        public bool Equals(Lesson other)
        {
            return Time.Date == other.Time.Date && StartEndTime == other.StartEndTime;
        }

        public override int GetHashCode()
        {
            return Time.GetHashCode() ^ StartEndTime.GetHashCode();
        }
    }
}
