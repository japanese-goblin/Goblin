using System.Linq;
using System.Threading.Tasks;
using Goblin.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vk;

namespace Goblin.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminApiController : Controller
    {
        private readonly BotDbContext _db;
        private readonly VkApi _api;

        public AdminApiController(BotDbContext db, VkApi api)
        {
            _db = db;
            _api = api;
        }

        public async Task<IActionResult> SendToAll(string msg, string[] attachs)
        {
            var gr = _db.GetUsers().Select(x => x.Vk).ToArray();
            await _api.Messages.Send(gr, msg, attachs);
            return RedirectToAction("Messages", "Admin");
        }

        public async Task<IActionResult> SendToId(long peerId, string msg, string[] attachs)
        {
            await _api.Messages.Send(peerId, msg, attachs);
            return RedirectToAction("Messages", "Admin");
        }
    }
}