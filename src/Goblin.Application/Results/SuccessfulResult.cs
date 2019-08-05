namespace Goblin.Application.Results
{
    public class SuccessfulResult : IResult
    {
        public bool IsSuccessful => true;

        public string Message { get; }
        public string[] Attachments { get; }
        public object Keyboard { get; } //TODO: change type
    }
}