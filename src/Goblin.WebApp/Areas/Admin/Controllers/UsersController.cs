using System.Linq;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly BotDbContext _db;

        public UsersController(BotDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var data = _db.VkBotUsers
                          .AsNoTracking()
                          .ToArray();
            ViewData["count"] = data.Length;
            return View(data.GroupBy(x => x.NarfuGroup));
        }
    }
}