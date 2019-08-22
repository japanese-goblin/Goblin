namespace Goblin.Application.Results.Failed
{
    public class CommandNotFoundResult : FailedResult
    {
        public CommandNotFoundResult() : base("Команда не найдена. Проверьте правильность написания команды.")
        {
        }
    }
}