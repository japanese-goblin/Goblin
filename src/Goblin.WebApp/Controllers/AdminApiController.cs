using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Controllers
{
    [Route("api/admin/[action]")]
    [Authorize(Roles = "Admin")]
    public class AdminApiController : Controller
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public AdminApiController(BotDbContext db, IVkApi vkApi)
        {
            _db = db;
            _vkApi = vkApi;
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
                    UserIds = chunk,
                    Attachments = new[] { new Wall() }
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

        [HttpPost]
        public async Task<IActionResult> AddRemind(long peerId, string text, string dateStr, string time)
        {
            var date = DateTime.Parse($"{dateStr} {time}");
            try
            {
                _db.Reminds.Add(new Remind(peerId, text, date));
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Log.ForContext<AdminApiController>().Error(ex, "Невозможно добавить напоминание");
            }

            return RedirectToAction("Reminds", "Admin");
        }
    }
}