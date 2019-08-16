using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using VkNet.Model;

namespace Goblin.Application.TextCommands
{
    public class SetDataCommand : ITextCommand
    {
        public bool IsAdminCommand => false;

        public string[] Aliases => new[] { "установить" };

        private readonly BotDbContext _db;
        private readonly OpenWeatherMapApi _weather;
        private readonly NarfuApi _narfu;

        public SetDataCommand(BotDbContext db, OpenWeatherMapApi weather, NarfuApi narfu)
        {
            _db = db;
            _weather = weather;
            _narfu = narfu;
        }

        public async Task<IResult> Execute(Message msg, BotUser user = null)
        {
            var botUser = _db.BotUsers.Find(user.VkId);
            var prms = msg.GetCommandParameters();
            if(prms.Length != 2)
            {
                return new FailedResult(new List<string>
                {
                    "Укажите 2 параметра команды. Пример использования: установить город Москва / установить группу 123456"
                });
            }

            if(prms[0] == "город")
            {
                var city = prms[1];
                var isExists = await _weather.IsCityExists(city);
                if(!isExists)
                {
                    return new FailedResult(new List<string>
                    {
                        $"Город {city} не найден"
                    });
                }

                botUser.SetCity(city);
                await _db.SaveChangesAsync();
                return new SuccessfulResult
                {
                    Message = $"Город успешно установлен на {city}"
                };
            }
            else if(prms[0] == "группу" || prms[0] == "группа")
            {
                var group = prms[1];
                if(!int.TryParse(group, out var intGroup))
                {
                    return new FailedResult(new List<string>
                    {
                        "Укажите корректный номер группы."
                    });
                }

                var isExists = _narfu.Students.IsCorrectGroup(intGroup);
                if(!isExists)
                {
                    return new FailedResult(new List<string>
                    {
                        $"Группа с номером {intGroup} не найдена."
                    });
                }

                user.SetNarfuGroup(intGroup);
                _db.SaveChanges();
                return new SuccessfulResult
                {
                    Message = $"Группа успешно установлена на {group}"
                };
            }

            return new FailedResult(new List<string>
            {
                "Укажите что вы хотите установить: группу или город. \n" +
                "Пример использования: установить город Москва / установить группу 123456"
            });
        }
    }
}