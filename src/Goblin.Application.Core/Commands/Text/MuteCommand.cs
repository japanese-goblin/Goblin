using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Text
{
    public class MuteCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "мут" };
        private readonly BotDbContext _db;

        public MuteCommand(BotDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            user = await _db.BotUsers.FindAsync(user.VkId);
            user.SetErrorNotification(!user.IsErrorsEnabled);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = user.IsErrorsEnabled ? "Ошибки включены" : "Ошибки отключены"
            };
        }
    }
}