using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;

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

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            user = _db.Entry(user).Entity;
            user.SetErrorNotification(!user.IsErrorsEnabled);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = user.IsErrorsEnabled ? "Ошибки включены" : "Ошибки отключены"
            };
        }
    }
}