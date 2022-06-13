namespace Goblin.WebApp.Responses;

public class ItemsResponse<T>
{
    public long Total { get; set; }
    public IEnumerable<T> Items { get; set; }
}