using System.Threading.Tasks;
using Goblin.Application.Vk.Extensions;
using Goblin.Domain;
using Goblin.WebApp.Hangfire;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MessagesController : Controller
    {
        private readonly IVkApi _vkApi;

        public MessagesController(IVkApi vkApi)
        {
            _vkApi = vkApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendToAll(string msg, string[] attachments, ConsumerType type)
        {
            BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToAll(msg, attachments, type));
            return RedirectToAction("Index", "Messages");
        }

        [HttpPost]
        public async Task<IActionResult> SendToId(long peerId, string msg, string[] attachments, ConsumerType type)
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