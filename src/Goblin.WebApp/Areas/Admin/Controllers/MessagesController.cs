using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MessagesController : Controller
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public MessagesController(BotDbContext db, IVkApi vkApi)
        {
            _db = db;
            _vkApi = vkApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendToAll(string msg)
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

            return RedirectToAction("Index", "Messages");
        }

        [HttpPost]
        public async Task<IActionResult> SendToId(long peerId, string msg)
        {
            await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = msg
            });

            return RedirectToAction("Index", "Messages");
        }
    }
}