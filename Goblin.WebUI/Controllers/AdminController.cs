using System.Linq;
using Goblin.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BotDbContext _context;

        public AdminController(BotDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.GetUsers().OrderBy(x => x.Id).ToList());
        }

        public IActionResult Messages()
        {
            return View();
        }
    }
}