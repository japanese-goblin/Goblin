namespace Goblin.Application.Core.Results.Failed
{
    public class CommandNotFoundResult : FailedResult
    {
        public CommandNotFoundResult() : base("Команда не найдена. Проверьте правильность написания команды. " +
                                              "Если вы хотите отключить подобные ошибки, то, пожалуйста, напишите команду 'мут'")
        {
        }
    }
}