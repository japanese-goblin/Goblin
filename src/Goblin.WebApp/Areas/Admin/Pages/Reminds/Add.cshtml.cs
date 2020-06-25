using System;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace Goblin.WebApp.Areas.Admin.Pages.Reminds
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Add : PageModel
    {
        public string ErrorMessage { get; set; }
        private readonly BotDbContext _context;

        public Add(BotDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(long peerId, ConsumerType consumerType, string text, string dateStr, string timeStr)
        {
            var isCorrectDate = DateTime.TryParse($"{dateStr} {timeStr}", out var date);
            if(!isCorrectDate)
            {
                ErrorMessage = "Некорректная дата";
                return Page();
            }

            try
            {
                await _context.Reminds.AddAsync(new Remind(peerId, text, date, consumerType));
                await _context.SaveChangesAsync();

                return RedirectToPage("Index", "Reminds");
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                Log.ForContext<Add>().Error(ex, "Невозможно добавить напоминание");
            }

            return Page();
        }
    }
}