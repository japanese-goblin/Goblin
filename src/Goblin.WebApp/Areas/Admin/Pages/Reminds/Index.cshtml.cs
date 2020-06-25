using System.Linq;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Goblin.WebApp.Areas.Admin.Pages.Reminds
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Index : PageModel
    {
        public Remind[] Reminds { get; set; }
        private readonly BotDbContext _context;

        public Index(BotDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Reminds = _context.Reminds.ToArray();
            ViewData["count"] = Reminds.Length;
        }
    }
}