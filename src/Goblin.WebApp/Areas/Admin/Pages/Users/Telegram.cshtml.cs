using System.Collections.Generic;
using System.Linq;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Areas.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Telegram : PageModel
    {
        public IEnumerable<IGrouping<int, TgBotUser>> Users { get; set; }
        private readonly BotDbContext _context;

        public Telegram(BotDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            var data = _context.TgBotUsers
                               .AsNoTracking()
                               .ToArray();
            ViewData["count"] = data.Length;

            Users = data.GroupBy(x => x.NarfuGroup);
        }
    }
}