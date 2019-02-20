using System;

namespace Narfu.Models
{
    public class Lesson : IEquatable<Lesson>
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StartEndTime { get; set; }
        public byte Number { get; set; }
        public string Address { get; set; }
        public string Auditory { get; set; }
        public string Teacher { get; set; }
        public string Groups { get; set; }

        public bool Equals(Lesson other)
        {
            return StartTime == other.StartTime && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ StartTime.GetHashCode();
        }
    }
}