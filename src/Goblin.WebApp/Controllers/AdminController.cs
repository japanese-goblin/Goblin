using System;
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
            return View(_db.BotUsers.Include(x => x.SubscribeInfo).ToArray());
        }
        
        public IActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendToAll(string msg, string[] attachs)
        {
            var gr = _db.BotUsers.Select(x => x.VkId).ToArray();
            await _vkApi.Messages.SendToUserIdsAsync(new MessagesSendParams
            {
                Message = msg,
                UserIds = gr,
                RandomId = new Random().Next(0, int.MaxValue)
            });
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