namespace Zidium
{
    internal static class LogLevelHelper
    {
        public static Api.LogLevel GetLogLevel(NLog.LogLevel level)
        {
            if (level.Ordinal >= NLog.LogLevel.Fatal.Ordinal)
                return  Api.LogLevel.Fatal;

            if (level.Ordinal >= NLog.LogLevel.Error.Ordinal)
                return Api.LogLevel.Error;

            if (level.Ordinal >= NLog.LogLevel.Warn.Ordinal)
                return Api.LogLevel.Warning;

            if (level.Ordinal >= NLog.LogLevel.Info.Ordinal)
                return Api.LogLevel.Info;

            if (level.Ordinal >= NLog.LogLevel.Debug.Ordinal)
                return Api.LogLevel.Debug;
            
            return Api.LogLevel.Trace;
        }

    }
}
