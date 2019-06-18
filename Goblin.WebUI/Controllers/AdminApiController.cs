using System.Linq;
using System.Threading.Tasks;
using Goblin.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vk;

namespace Goblin.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminApiController : Controller
    {
        private readonly BotDbContext _db;
        private readonly VkApi _api;
        private readonly ILogger<AdminApiController> _logger;

        public AdminApiController(BotDbContext db, VkApi api, ILogger<AdminApiController> logger)
        {
            _db = db;
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> SendToAll(string msg, string[] attachs)
        {
            _logger.LogWarning("Отправка сообщения всем пользователям");
            var gr = _db.GetUsers().Select(x => x.Vk).ToArray();
            await _api.Messages.Send(gr, msg, attachs);
            return RedirectToAction("Messages", "Admin");
        }

        public async Task<IActionResult> SendToId(long peerId, string msg, string[] attachs)
        {
            _logger.LogInformation("Отправка сообщения с id {0}", peerId);
            await _api.Messages.Send(peerId, msg, attachs);
            return RedirectToAction("Messages", "Admin");
        }
    }
}