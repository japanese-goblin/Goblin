using System.Linq;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BotDbContext _db;

        public AdminController(BotDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var data = _db.BotUsers.Include(x => x.SubscribeInfo).AsNoTracking();
            ViewData["count"] = data.Count();
            return View(data.AsEnumerable().GroupBy(x => x.NarfuGroup));
        }

        public IActionResult Messages()
        {
            return View();
        }

        public IActionResult Reminds()
        {
            var data = _db.Reminds.ToArray();
            ViewData["count"] = data.Length;
            return View(data);
        }

        public IActionResult AddRemind()
        {
            return View();
        }
    }
}