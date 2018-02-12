using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        private MainContext db;
        public ApiController(MainContext context)
        {
            db = context;
        }
        public bool SendMessage(string msg)
        {
            return Utils.SendMessage(db.Users.Select(x => x.Vk).ToList(), msg);
        }


        //public JsonResult Users()
        //{
        //    return Json(db.Users);
        //}
    }
}