using System;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Areas.Admin.Pages.CronJobs
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Create : PageModel
    {
        [BindProperty]
        [FromForm]
        public InputModel Input { get; set; }
        private readonly BotDbContext _context;

        public Create(BotDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPost()
        {
            var job = new CronJob(Input.Name, Input.ChatId, Input.NarfuGroup, Input.WeatherCity, Input.Hours, Input.Minutes,
                                  Input.ConsumerType);
            await _context.AddAsync(job);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("Index");
        }
    }

    public class InputModel
    {
        public string Name { get; set; }
        public long ChatId { get; set; }

        public int NarfuGroup { get; set; }
        public string WeatherCity { get; set; }

        public int Hours { get; set; }
        public int Minutes { get; set; }

        public ConsumerType ConsumerType { get; set; }
    }
}