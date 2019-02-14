using System.Linq;
using System.Threading.Tasks;
using Goblin.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Vk;

namespace Goblin.Controllers
{
    public class AdminApiController : Controller
    {
        private readonly MainContext _db;
        private readonly VkApi _api;

        public AdminApiController(MainContext db, VkApi api)
        {
            _db = db;
            _api = api;
        }

        public async Task SendToAll(string msg, string[] attach)
        {
            var gr = _db.GetUsers().Select(x => x.Vk).ToArray();
            await _api.Messages.Send(gr, msg, attach);
        }

        public async Task SendToId(long id, string msg, string[] attachs)
        {
            await _api.Messages.Send(id, msg, attachs);
        }
    }
}