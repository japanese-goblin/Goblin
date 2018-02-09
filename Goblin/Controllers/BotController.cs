using Microsoft.AspNetCore.Mvc;
using Goblin.Bot;
using Goblin.Models;

namespace Goblin.Controllers
{
    public class BotController : Controller
    {
        MainContext db;

        public BotController(MainContext context)
        {
            db = context;
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
                    Utils.SendMessage(userID, CommandsList.ExecuteCommand(msg));
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    //TODO: add db
                    //db.Persons.Add(new Person() {VkID = userID});
                    //db.SaveChanges();
                    //{"type":"group_join","object":{"user_id":***REMOVED***,"join_type":"join"},"group_id":146286422}
                    Utils.SendMessage(Utils.DevelopersID, $"@id{userID} ({Utils.GetUserName(userID)}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    //{"type":"group_leave","object":{"user_id":***REMOVED***,"self":1},"group_id":146286422}
                    //{"type":"message_deny","object":{"user_id":***REMOVED***},"group_id":146286422}
 
                    Utils.SendMessage(Utils.DevelopersID, $"@id{userID} ({Utils.GetUserName(userID)}) отписался!");
                    break;
            }
            return "ok";
        }
    }
}