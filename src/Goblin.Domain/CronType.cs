using System;

namespace Goblin.Domain
{
    [Flags]
    public enum CronType
    {
        Weather,
        Schedule,
        Text
    }
}