using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Goblin.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            //var x = ZidiumHelper.GetSystemControl();
            //x.AddApplicationError("darova", "aslamsd");
            var log = LogManager.GetCurrentClassLogger();
            log.Error("goblin");
            log.Info("info");
            log.Debug("debug");
            log.Trace("trace");
        }
    }
}