using Microsoft.AspNetCore.Mvc;
using Goblin.Bot;
using Goblin.Models;
using System.Linq;

namespace Goblin.Controllers
{
    public class BotController : Controller
    {
        MainContext db;

        public BotController(MainContext context)
        {
            db = context;
            //TODO: так плохо делать
            Utils.DB = db;
        }

        public string Handler([FromBody] dynamic body)
        {
            var eventType = body["type"].ToString();
            int userID;
            switch (eventType)
            {
                case "confirmation":
                    return Utils.ConfirmationToken;

                case "message_new":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    var msg = body["object"]["body"].ToString();
                    if(!db.Users.Any(x => x.Vk == userID))
                    {
                        db.Users.Add(new User() { Vk = userID });
                        db.SaveChanges();
                    }
                    Utils.SendMessage(userID, CommandsList.ExecuteCommand(msg, userID));
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    db.Users.Add(new User() { Vk = userID });
                    db.SaveChanges();
                    //{"type":"group_join","object":{"user_id":***REMOVED***,"join_type":"join"},"group_id":146286422}
                    Utils.SendMessage(Utils.DevelopersID, $"@id{userID} ({Utils.GetUserName(userID)}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    //{"type":"group_leave","object":{"user_id":***REMOVED***,"self":1},"group_id":146286422}
                    //{"type":"message_deny","object":{"user_id":***REMOVED***},"group_id":146286422}
                    if (db.Users.Any(x => x.Vk == userID))
                    {
                        db.Users.Remove(db.Users.First(x => x.Vk == userID));
                        db.SaveChanges();
                    }
                    Utils.SendMessage(Utils.DevelopersID, $"@id{userID} ({Utils.GetUserName(userID)}) отписался!");
                    break;
            }
            return "ok";
        }
    }
}