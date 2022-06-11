using FastEndpoints;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.WebApp.Responses;
using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Endpoints.Admin.Users;

public class GetUsers : Endpoint<GetUsersRequest, ItemsResponse<BotUserDto>>
{
    private readonly BotDbContext _context;

    public GetUsers(BotDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/admin/users");
    }

    public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
    {
        var users = await _context.BotUsers.Where(x => x.ConsumerType == req.Consumer)
                                  .Select(x => new BotUserDto
                                  {
                                      Id = x.Id,
                                      NarfuGroup = x.NarfuGroup,
                                      WeatherCity = x.WeatherCity,
                                      IsAdmin = x.IsAdmin,
                                      HasScheduleSubscription = x.HasScheduleSubscription,
                                      HasWeatherSubscription = x.HasWeatherSubscription
                                  })
                                  .ToArrayAsync(ct);
        await SendOkAsync(new ItemsResponse<BotUserDto>
        {
            Total = users.Length,
            Items = users
        }, ct);
    }
}

public class GetUsersRequest
{
    public ConsumerType Consumer { get; set; }
}