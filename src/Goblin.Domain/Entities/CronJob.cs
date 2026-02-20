namespace Goblin.Domain.Entities;

public class CronJob
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public long ChatId { get; private set; }

    public int? NarfuGroup { get; private set; }
    public string? WeatherCity { get; private set; }
    public string? Text { get; set; }

    public CronTime Time { get; private set; }
    public CronType CronType { get; set; }

    public ConsumerType ConsumerType { get; private set; }

    protected CronJob()
    {
    }

    public CronJob(string name, long chatId, int narfuGroup, string weatherCity, CronTime time, ConsumerType consumerType,
                   CronType cronType, string text = "")
    {
        SetName(name);
        SetChatId(chatId);
        SetNarfuGroup(narfuGroup);
        SetWeatherCity(weatherCity);
        SetConsumerType(consumerType);
        SetCronTime(time);
        SetCronType(cronType);
        SetText(text);
    }

    public void SetName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Параметр должен быть непустым", nameof(name));
        }

        Name = name;
    }

    public void SetChatId(long chatId)
    {
        if(chatId <= 0)
        {
            throw new ArgumentException("Параметр должен быть больше 0", nameof(chatId));
        }

        ChatId = chatId;
    }

    public void SetNarfuGroup(int group)
    {
        if(group < 0)
        {
            throw new ArgumentException("Параметр должен быть больше 0", nameof(group));
        }

        NarfuGroup = group;
    }

    public void SetWeatherCity(string city)
    {
        WeatherCity = city;
    }

    private void SetConsumerType(ConsumerType type)
    {
        ConsumerType = type;
    }

    private void SetCronType(CronType type)
    {
        CronType = type;
    }

    private void SetText(string text)
    {
        Text = text;
    }

    public void SetCronTime(CronTime time)
    {
        Time = time;
    }
}