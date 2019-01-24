using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Common;
using Zidium.Api;

namespace Zidium
{
    [Target("Zidium.Errors")]
    public class NLogTargetErrors : Target
    {
        protected override void Write(LogEventInfo logEvent)
        {
            var level = LogLevelHelper.GetLogLevel(logEvent.Level);
            if (level <= Api.LogLevel.Info)
                return;

            string message;

            if (logEvent.Exception != null && logEvent.Message == "{0}")
                message = logEvent.Exception.Message;
            else
                message = logEvent.FormattedMessage;

            Dictionary<string, object> properties;

            if (logEvent.HasProperties)
            {
                properties = logEvent.Properties.ToDictionary(a => a.Key.ToString(), b => b.Value);
            }
            else
            {
                properties = new Dictionary<string, object>();
            }

            var errorData = Component.Client.ExceptionRender.CreateEventFromLog(Component, level, logEvent.Exception, message, properties);
            errorData.Add();
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
            Client.Instance.EventManager.Flush();
            base.FlushAsync(asyncContinuation);
        }
    }
}
