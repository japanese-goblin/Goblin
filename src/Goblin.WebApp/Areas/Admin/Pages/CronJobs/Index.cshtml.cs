using System;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Goblin.WebApp.Hangfire;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Areas.Admin.Pages.CronJobs
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Index : PageModel
    {
        private readonly BotDbContext _context;

        public CronJob[] CronJobs { get; set; }

        public Index(BotDbContext context)
        {
            _context = context;
        }

        public async Task OnGet()
        {
            CronJobs = await _context.CronJobs.ToArrayAsync();
        }
    }
}