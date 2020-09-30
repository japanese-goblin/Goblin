using System;

namespace Goblin.Domain
{
    [Flags]
    public enum CronType
    {
        Weather = 1,
        Schedule = 2,
        Text = 4
    }
}