using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Vk;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class AdminApiController : Controller
    {
        public async Task SendToAll(string msg, string[] attach)
        {
            var gr = DbHelper.GetUsers().Select(x => x.Vk).ToArray();
            await VkMethods.SendMessage(gr, msg, attach);
        }

        public async Task SendToId(long id, string msg, string[] attach)
        {
            await VkMethods.SendMessage(id, msg, attach);
        }
    }
}