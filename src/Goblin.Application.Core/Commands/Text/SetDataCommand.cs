using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Goblin.Narfu;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap;
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

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            user = _db.Entry(user).Entity;
            var prms = msg.Text.Split(' ', 3);
            if(prms.Length != 3)
            {
                return new FailedResult("Укажите 2 параметра команды." +
                                        "Пример использования: установить город Москва / установить группу 123456");
            }

            if(prms[1].Equals("город", StringComparison.OrdinalIgnoreCase))
            {
                return await SetCity(prms, user);
            }

            if(prms[1].Equals("группу", StringComparison.OrdinalIgnoreCase) ||
               prms[1].Equals("группа", StringComparison.OrdinalIgnoreCase))
            {
                return await SetGroup(user, prms);
            }

            return new FailedResult("Укажите что вы хотите установить: группу или город. \n" +
                                    "Пример использования: установить город Москва / установить группу 123456");
        }

        private async Task<IResult> SetGroup(BotUser user, string[] prms)
        {
            var group = prms[2];
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

        private async Task<IResult> SetCity(string[] prms, BotUser user)
        {
            var city = prms[2].ToUpperFirstLetter();
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