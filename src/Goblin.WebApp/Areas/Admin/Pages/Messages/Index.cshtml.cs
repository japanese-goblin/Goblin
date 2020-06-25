﻿using System;
using Goblin.Domain;
using Goblin.WebApp.Hangfire;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Goblin.WebApp.Areas.Admin.Pages.Messages
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class Index : PageModel
    {
        public void OnPostSendToAll(string msg, string[] attachments, ConsumerType type, string isSendKeyboard)
        {
            var isKeyboard = isSendKeyboard.Equals("on", StringComparison.OrdinalIgnoreCase) ? true : false;
            BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToAll(msg, attachments, isKeyboard, type));
        }

        public void OnPostSendToId(long chatId, string msg, string[] attachments, ConsumerType type)
        {
            BackgroundJob.Enqueue<SendToUsersTasks>(x => x.SendToId(chatId, msg, attachments, type));
        }
    }
}