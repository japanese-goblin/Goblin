using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Commands.Text
{
    public class ChooseCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "выбери", "рандом" };

        public Task<IResult> Execute<T>(Message msg, BotUser user) where T : BotUser
        {
            var param = string.Join(' ', msg.CommandParameters);
            var split = Split(param);

            if(split.Length < 2)
            {
                const string text = "Введите два или более предложений, разделенных следующими символами: ',' и 'или'";
                return Task.FromResult<IResult>(new FailedResult(text));
            }

            var random = GetRandom(0, split.Length);

            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = $"Я выбираю это: {split[random]}"
            });
        }

        private static int GetRandom(int start, int end)
        {
            return new Random(DateTime.Now.Millisecond).Next(start, end);
        }

        private static string[] Split(string str)
        {
            return str.Split(new[] { ",", ", ", " или " }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}