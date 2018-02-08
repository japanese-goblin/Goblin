using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class BotController : Controller
    {
        public string Handler([FromBody] dynamic body)
        {
            var eventType = body["type"].ToString();
            var a = body["object"];
            var userID = int.Parse(a["user_id"].ToString());
            switch (eventType)
            {
                case "confirmation":
                    return Utils.ConfirmationToken;

                case "message_new":
                    Utils.SendMessage(userID, "privet");
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