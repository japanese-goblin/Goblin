using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Goblin.Bot;

namespace Goblin.Controllers
{
    public class BotController : Controller
    {
        public string Handler([FromBody] dynamic body)
        {
            var eventType = body["type"].ToString();
            switch (eventType)
            {
                case "confirmation":
                    return Utils.ConfirmationToken;

                case "message_new":
                    var userID = int.Parse(body["object"]["user_id"].ToString());
                    var msg = body["object"]["body"].ToString();
                    Utils.SendMessage(userID, CommandsList.ExecuteCommand(msg));
                    break;

                case "group_join":
                    //send message
                    break;

                case "group_leave":
                case "message_deny":
                    //delete from db, send message
                    break;
            }
            //var aa = JsonConvert.DeserializeObject<dynamic>(Json as string);
            return "ok";
        }
    }
}