using System.Threading.Tasks;
using Vk;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Qoute : ICommand
    {
        public string Name { get; } = "Цитата";
        public string Decription { get; } = "Создает картинку из пересланного сообщения";
        public string Usage { get; } = "Цитата";
        public string[] Allias { get; } = {"цитата"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if (!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            var forwarded = msg.ForwardMessages[0];
            var user = await VkApi.Users.Get(forwarded.FromId);

            var image = await QuotesGenerator.Generator.GenerateQuote(forwarded.Text, forwarded.FromId,
                user.ToString(), user.Photo200Orig);
            var attach = await VkApi.Photos.FastUploadPhoto(msg.FromId, image); //TODO change fromId

            return new CommandResponse
            {
                Text = "Держите",
                Attachments = new [] {attach}
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if (msg.ForwardMessages.Length == 0)
            {
                return (false, "Ошибка. Перешлите хотя бы одно сообщение");
            }

            if (string.IsNullOrEmpty(msg.ForwardMessages[0].Text))
            {
                return (false, "Ошибка. Пересланное сообщение пустое");
            }

            return (true, "");
        }
    }
}