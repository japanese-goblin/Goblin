using System;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goblin.WebApp.Areas.Admin.Pages.Reminds;

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

    public async Task<IActionResult> OnPost(long chatId, ConsumerType consumerType, string text, string dateStr, string timeStr)
    {
        var isExists = await IsCorrectId(chatId, consumerType);
        if(!isExists)
        {
            ErrorMessage = $"Пользователь [{chatId}-{consumerType}] не существует в бд";
            return Page();
        }

        var isCorrectDate = DateTime.TryParse($"{dateStr} {timeStr}", out var date);
        if(!isCorrectDate)
        {
            ErrorMessage = "Некорректная дата";
            return Page();
        }

        try
        {
            await _context.Reminds.AddAsync(new Remind(chatId, text, date, consumerType));
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        catch(Exception ex)
        {
            ErrorMessage = ex.Message;
            Log.ForContext<Add>().Error(ex, "Невозможно добавить напоминание");
        }

        return Page();
    }

    [NonHandler]
    public async Task<bool> IsCorrectId(long chatId, ConsumerType type)
    {
        if(type == ConsumerType.Telegram)
        {
            var isExists = await _context.TgBotUsers.SingleOrDefaultAsync(x => x.Id == chatId);
            return isExists != null;
        }

        if(type == ConsumerType.Vkontakte)
        {
            var isExists = await _context.VkBotUsers.SingleOrDefaultAsync(x => x.Id == chatId);
            return isExists != null;
        }

        return false;
    }
}