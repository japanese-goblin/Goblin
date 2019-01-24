using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;
using LogLevel = NLog.LogLevel;

namespace Goblin
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger;
 
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }
 
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var isFile = path.EndsWith(".js") || path.EndsWith(".css");
            if (!isFile)
            {
                var logEvent = new LogEventInfo(LogLevel.Info, _logger.Name, $"Page visit: {path}");
                logEvent.Properties["Path"] = $"[{context.Request.Method}] {context.Request.Path.Value} - {context.Response.StatusCode}";
                logEvent.Properties["Ip"] = context.Connection.RemoteIpAddress;
                _logger.Log(logEvent);
            }

            await _next.Invoke(context);
        }
    }
}