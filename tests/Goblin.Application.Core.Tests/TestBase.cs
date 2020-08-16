using System;
using System.Collections.Generic;
using System.Text.Json;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Models;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Tests
{
    public class TestBase
    {
        public readonly VkBotUser AdminUser;
        public readonly BotDbContext ApplicationContext;
        public readonly VkBotUser DefaultUser;
        public readonly VkBotUser DefaultUserWithMaxReminds;

        public TestBase()
        {
            DefaultUser = new VkBotUser(1, "Архангельск", 351917, false, true, true, true);
            DefaultUserWithMaxReminds = new VkBotUser(2, "Архангельск", 351917, false, true, true, true);
            AdminUser = new VkBotUser(101010, "Архангельск", 351917, true, true, true, true);

            ApplicationContext = GetDbContext();
        }

        public BotDbContext GetDbContext()
        {
            if(ApplicationContext != null)
            {
                return ApplicationContext;
            }

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BotDbContext>()
                          .UseSqlite(connection)
                          .Options;

            var context = new BotDbContext(options);

            context.Database.EnsureCreated();

            InitDbContext(context);

            return context;
        }

        private void InitDbContext(BotDbContext context)
        {
            context.VkBotUsers.Add(DefaultUser);
            context.VkBotUsers.Add(AdminUser);
            context.VkBotUsers.Add(DefaultUserWithMaxReminds);

            for(var i = 0; i < AddRemindCommand.MaxRemindsCount; i++)
            {
                context.Reminds.Add(new Remind(DefaultUserWithMaxReminds.Id, "text", new DateTime(2101, 1, 1, 1, 1, 1),
                                               ConsumerType.Vkontakte));
            }

            context.SaveChanges();
        }

        public Message GenerateMessage(long userId, long chatId, string text = "")
        {
            return new Message
            {
                UserId = userId,
                ChatId = chatId,
                Text = text,
                Payload = string.Empty
            };
        }

        public Message GenerateMessageWithPayload(long userId, long chatId, string key, string value)
        {
            var dict = new Dictionary<string, string>
            {
                [key] = value
            };
            return new Message
            {
                UserId = userId,
                ChatId = chatId,
                Text = string.Empty,
                Payload = JsonSerializer.Serialize(dict)
            };
        }
    }
}