// using System;
// using System.Globalization;
// using System.Threading.Tasks;
// using Goblin.Application.Core.Abstractions;
// using Goblin.Application.Core.Results.Failed;
// using Goblin.Application.Core.Results.Success;
// using Goblin.DataAccess;
// using Goblin.Domain;
// using Goblin.Domain.Abstractions;
// using Goblin.Domain.Entities;
//
// namespace Goblin.Application.Core.Commands.Text
// {
//     public class AddRemindCommand : ITextCommand
//     {
//         private const int MaxRemindsCount = 8;
//
//         public bool IsAdminCommand => false;
//         public string[] Aliases => new[] { "напомни" };
//         private readonly BotDbContext _db;
//
//         public AddRemindCommand(BotDbContext db)
//         {
//             _db = db;
//         }
//
//         public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
//         {
//             var param = string.Join(' ', msg.MessageParams);
//             var all = param.Split(' ', 3);
//
//             if(!user.IsAdmin && user.Reminds.Count == MaxRemindsCount)
//             {
//                 return new FailedResult($"Вы уже достигли максимального количества напоминаний ({MaxRemindsCount})");
//             }
//
//             if(all.Length != 3)
//             {
//                 return new FailedResult("Укажите дату, время и текст напоминания (11.11.2011 11:11 текст)");
//             }
//
//             if(all[0].Equals("завтра", StringComparison.OrdinalIgnoreCase))
//             {
//                 var d = DateTime.Now.AddDays(1);
//                 all[0] = $"{d.Day}.{d.Month}.{d.Year}";
//             }
//             else if(all[0].Equals("сегодня", StringComparison.OrdinalIgnoreCase))
//             {
//                 var d = DateTime.Now;
//                 all[0] = $"{d.Day}.{d.Month}.{d.Year}";
//             }
//
//             var isCorrectTime = ParseTime(all[0], all[1], out var dateTime);
//             if(!isCorrectTime)
//             {
//                 return new FailedResult("Некорректная дата или время");
//             }
//
//             if(dateTime <= DateTime.Now)
//             {
//                 return new FailedResult("Дата напоминания меньше текущей");
//             }
//
//             await AddRemind<T>(user, all, dateTime);
//
//             return new SuccessfulResult
//             {
//                 Message = $"Окей. {dateTime:f} напомню следующее:\n{all[2]}"
//             };
//         }
//
//         private async Task AddRemind<T>(BotUser user, string[] all, DateTime dateTime) where T : BotUser
//         {
//             var type = ConsumerType.Vkontakte;
//             if(typeof(T) == typeof(TgBotUser))
//             {
//                 type = ConsumerType.Telegram;
//             }
//             else if(typeof(T) == typeof(VkBotUser))
//             {
//                 type = ConsumerType.Vkontakte;
//             }
//             
//             await _db.Reminds.AddAsync(new Remind(user.Id, all[2], dateTime, type));
//             await _db.SaveChangesAsync();
//         }
//
//         private static bool ParseTime(string date, string time, out DateTime dateTime)
//         {
//             var isCorrect = DateTime.TryParseExact($"{date} {time}",
//                                                    new[]
//                                                    {
//                                                        "dd.MM.yyyy HH:mm", "d.MM.yyyy HH:mm",
//                                                        "dd.M.yyyy HH:mm", "d.M.yyyy HH:mm",
//                                                        "dd.MM.yyyy H:mm", "d.MM.yyyy H:mm",
//                                                        "dd.M.yyyy H:mm", "d.M.yyyy H:mm",
//                                                        "dd.MM.yyyy HH:m", "d.MM.yyyy HH:m",
//                                                        "dd.M.yyyy HH:m", "d.M.yyyy HH:m",
//                                                        "dd.MM.yyyy H:m", "d.MM.yyyy H:m",
//                                                        "dd.M.yyyy H:m", "d.M.yyyy H:m"
//                                                    },
//                                                    null, DateTimeStyles.AssumeLocal, out dateTime);
//
//             return isCorrect;
//         }
//     }
// }

