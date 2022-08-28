namespace Goblin.WebApp.Responses;

public class BotUserDto
{
    public long Id { get; set; }

    public string WeatherCity { get; set; }
    public int NarfuGroup { get; set; }
    
    public bool IsAdmin { get; set; }
    public bool HasWeatherSubscription { get; set; }
    public bool HasScheduleSubscription { get; set; }
}