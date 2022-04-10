using FastEndpoints;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.WebApp.Endpoints.Schedule;

public class Get : Endpoint<ScheduleRequest, ScheduleResponse>
{
    private readonly INarfuApi _narfuApi;

    public Get(INarfuApi narfuApi)
    {
        _narfuApi = narfuApi;
    }

    public override void Configure()
    {
        Get("/api/schedule/{groupId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var r = await _narfuApi.Students.GetSchedule(req.GroupId, req.Date);
        await SendAsync(new ScheduleResponse
        {
            Lessons = r.GroupBy(x => x.StartTime.Date)
                       .ToDictionary(x => x.Key, x => x.AsEnumerable()),
            GroupId = req.GroupId,
            GroupName = _narfuApi.Students.GetGroupByRealId(req.GroupId).Name
        });
    }
}

public class ScheduleRequest
{
    [FromQuery]
    public int GroupId { get; set; }

    [FromQuery]
    public DateTime? Date { get; set; }
}

public class ScheduleResponse
{
    public string GroupName { get; set; }
    public int GroupId { get; set; }
    public Dictionary<DateTime, IEnumerable<Lesson>> Lessons { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}