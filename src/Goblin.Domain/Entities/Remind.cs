namespace Goblin.Domain.Entities;

public class Remind
{
    public Guid Id { get; private set; }

    public string Text { get; private set; }
    public DateTimeOffset Date { get; private set; }

    public Guid BotUserId { get; set; }
    public BotUser BotUser { get; set; }

    protected Remind()
    {
    }

    public Remind(Guid botUserId, string text, DateTimeOffset date)
    {
        BotUserId = botUserId;
        SetText(text);
        SetDateTime(date);
    }

    private void SetText(string text)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Параметр должен быть непустым", nameof(text));
        }

        Text = text;
    }

    private void SetDateTime(DateTimeOffset date)
    {
        if(date < DateTime.Now)
        {
            throw new ArgumentException("Дата должна быть больше текущей", nameof(date));
        }

        Date = date.ToUniversalTime();
    }
}