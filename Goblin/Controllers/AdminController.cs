using System.Linq;
using Goblin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private MainContext db;
        public AdminController(MainContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            ViewBag.Users = db.Users.ToList();
            return View();
        }
    }
}