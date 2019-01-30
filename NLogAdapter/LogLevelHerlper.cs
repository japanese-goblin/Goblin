using Zidium.Api;

namespace Zidium
{
    internal static class LogLevelHelper
    {
        public static LogLevel GetLogLevel(NLog.LogLevel level)
        {
            if(level.Ordinal >= NLog.LogLevel.Fatal.Ordinal)
                return LogLevel.Fatal;

            if(level.Ordinal >= NLog.LogLevel.Error.Ordinal)
                return LogLevel.Error;

            if(level.Ordinal >= NLog.LogLevel.Warn.Ordinal)
                return LogLevel.Warning;

            if(level.Ordinal >= NLog.LogLevel.Info.Ordinal)
                return LogLevel.Info;

            if(level.Ordinal >= NLog.LogLevel.Debug.Ordinal)
                return LogLevel.Debug;

            return LogLevel.Trace;
        }
    }
}
