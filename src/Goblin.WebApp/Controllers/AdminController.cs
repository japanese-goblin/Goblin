using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public AdminController(BotDbContext db, IVkApi vkApi)
        {
            _db = db;
            _vkApi = vkApi;
        }

        public IActionResult Index()
        {
            var data = _db.BotUsers.Include(x => x.SubscribeInfo).AsNoTracking();
            ViewData["count"] = data.Count();
            return View(data.AsEnumerable().GroupBy(x => x.NarfuGroup));
        }

        public IActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendToAll(string msg, string[] attachs)
        {
            var chunks = _db.BotUsers.AsNoTracking().Select(x => x.VkId)
                        .AsEnumerable().Chunk(100);
            
            foreach(var chunk in chunks)
            {
                await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                {
                    Message = msg,
                    UserIds = chunk
                });
            }
            
            return RedirectToAction("Messages", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> SendToId(long peerId, string msg, string[] attachs)
        {
            await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = msg
            });
            return RedirectToAction("Messages", "Admin");
        }
    }
}