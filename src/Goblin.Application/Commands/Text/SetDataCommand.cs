using System;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using VkNet.Model;

namespace Goblin.Application.Commands.Text
{
    public class SetDataCommand : ITextCommand
    {
        private readonly BotDbContext _db;
        private readonly NarfuApi _narfu;
        private readonly OpenWeatherMapApi _weather;

        public SetDataCommand(BotDbContext db, OpenWeatherMapApi weather, NarfuApi narfu)
        {
            _db = db;
            _weather = weather;
            _narfu = narfu;
        }

        public bool IsAdminCommand => false;

        public string[] Aliases => new[] { "установить" };

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var botUser = _db.BotUsers.Find(user.VkId);
            var prms = msg.Text.Split(' ', 3);
            if(prms.Length != 3)
            {
                return new FailedResult("Укажите 2 параметра команды." +
                                        "Пример использования: установить город Москва / установить группу 123456");
            }

            if(prms[1] == "город")
            {
                var city = prms[2].ToUpperFirstLetter();
                var isExists = await _weather.IsCityExists(city);
                if(!isExists)
                {
                    return new FailedResult($"Город {city} не найден");
                }

                botUser.SetCity(city);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = $"Город успешно установлен на {city}"
                };
            }

            if(prms[1].Equals("группу", StringComparison.CurrentCultureIgnoreCase) ||
               prms[1].Equals("группа", StringComparison.CurrentCultureIgnoreCase))
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

            return new FailedResult("Укажите что вы хотите установить: группу или город. \n" +
                                    "Пример использования: установить город Москва / установить группу 123456");
        }
    }
}