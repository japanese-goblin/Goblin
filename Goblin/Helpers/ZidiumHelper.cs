using Zidium.Api;
using Zidium.Api.XmlConfig;

namespace Goblin.Helpers
{
    public static class ZidiumHelper
    {
        private static IClient _client;
        private static IComponentControl _componentControl;

        private static IClient GetClient()
        {
            if (_client != null) return _client;
            lock (typeof(ZidiumHelper))
            {
                if (_client != null) return _client;

                var zidiumConfig = ConfigHelper.LoadFromXmlOrGetDefault();
                _client = new Client(zidiumConfig);
            }
            return _client;
        }

        public static IComponentControl GetSystemControl()
        {
            if (_componentControl == null)
            {
                _componentControl = GetClient().GetRootComponentControl().GetOrCreateChildComponentControl("System.ComponentTypes.WebSite", "ASP.NET Goblin ");
            }
            return _componentControl;
        }
    }
}