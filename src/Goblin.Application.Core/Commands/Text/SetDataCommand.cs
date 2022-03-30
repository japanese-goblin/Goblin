using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;

namespace Goblin.Application.Core.Commands.Text
{
    public class SetDataCommand : ITextCommand
    {
        public bool IsAdminCommand => false;

        public string[] Aliases => new[] { "установить" };
        private readonly BotDbContext _db;
        private readonly INarfuApi _narfu;
        private readonly IOpenWeatherMapApi _weather;

        public SetDataCommand(BotDbContext db, IOpenWeatherMapApi weather, INarfuApi narfu)
        {
            _db = db;
            _weather = weather;
            _narfu = narfu;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var parameters = msg.Text.Split(' ', 3)[1..];
            user = _db.Entry(user).Entity;
            if(parameters.Length < 2)
            {
                return new FailedResult("Укажите 2 параметра команды." +
                                        "Пример использования: установить город Москва / установить группу 123456");
            }

            var whatToSet = parameters[0];
            var dataToSet = parameters[1];

            if(whatToSet.Contains("город", StringComparison.InvariantCultureIgnoreCase))
            {
                return await SetCity(dataToSet, user);
            }

            if(whatToSet.Contains("групп", StringComparison.InvariantCultureIgnoreCase))
            {
                return await SetGroup(dataToSet, user);
            }

            return new FailedResult("Укажите что вы хотите установить: группу или город. \n" +
                                    "Пример использования: установить город Москва / установить группу 123456");
        }

        private async Task<IResult> SetGroup(string group, BotUser user)
        {
            if(!int.TryParse(group, out var intGroup))
            {
                return new FailedResult("Укажите корректный номер группы.");
            }

            var isExists = _narfu.Students.IsCorrectGroup(intGroup);
            if(!isExists)
            {
                return new FailedResult($"Группа с номером {intGroup} не найдена.");
            }

            var groupName = _narfu.Students.GetGroupByRealId(intGroup).Name;

            user.SetNarfuGroup(intGroup);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = $"Группа успешно установлена на {intGroup} ({groupName})"
            };
        }

        private async Task<IResult> SetCity(string city, BotUser user)
        {
            city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city);
            var isExists = await _weather.IsCityExists(city);
            if(!isExists)
            {
                return new FailedResult($"Город {city} не найден");
            }

            user.SetCity(city);
            await _db.SaveChangesAsync();
            return new SuccessfulResult
            {
                Message = $"Город успешно установлен на {city}"
            };
        }
    }
}