using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Common;
using Zidium.Api;

namespace Zidium
{
    [Target("Zidium.Log")]
    public class NLogTargetLog : Target
    {
        protected override void Write(LogEventInfo logEvent)
        {
            var log = Component.Log.GetTaggedCopy(logEvent.LoggerName);

            string message;

            if (logEvent.Exception != null && logEvent.Message == "{0}")
                message = logEvent.Exception.Message;
            else
                message = logEvent.FormattedMessage;

            Dictionary<string, object> properties = null;

            if (logEvent.HasProperties)
            {
                properties = logEvent.Properties.ToDictionary(a => a.Key.ToString(), b => b.Value);
            }

            var level = LogLevelHelper.GetLogLevel(logEvent.Level);

            log.Write(level, message, logEvent.Exception, properties);
        }

        private IComponentControl _component;

        private IComponentControl Component
        {
            get
            {
                if (_component == null)
                {
                    _component = _componentId != null ? Client.Instance.GetComponentControl(_componentId.Value) : Client.Instance.GetDefaultComponentControl();
                }
                return _component;
            }
        }

        private Guid? _componentId;

        public string ComponentId
        {
            get
            {
                return _componentId != null ? _componentId.ToString() : null;
            }
            set
            {
                _componentId = !string.IsNullOrEmpty(value) ? new Guid(value) : (Guid?)null;
            }
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            Client.Instance.WebLogManager.Flush();
            base.FlushAsync(asyncContinuation);
        }
    }
}
