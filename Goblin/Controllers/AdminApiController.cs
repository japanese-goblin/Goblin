﻿using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.AspNetCore.Mvc;
using Vk;

namespace Goblin.Controllers
{
    public class AdminApiController : Controller
    {
        public async Task SendToAll(string msg, string[] attach)
        {
            var gr = DbHelper.GetUsers().Select(x => x.Vk).ToArray();
            await VkApi.Messages.Send(gr, msg, attach);
        }

        public async Task SendToId(long id, string msg, string[] attachs)
        {
            await VkApi.Messages.Send(id, msg, attachs);
        }
    }
}
