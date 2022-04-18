﻿using FastEndpoints;
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
        var dict = r.OrderBy(x => x.StartTime.Date)
                          .ThenBy(x => x.Number)
                          .GroupBy(x => GetStartOfWeek(x.StartTime));
        await SendAsync(new ScheduleResponse
        {
            Lessons = dict.ToDictionary(x => x.Key,
                                        x => x.GroupBy(y => y.StartTime.Date)
                                              .ToDictionary(y => y.Key, y => y.AsEnumerable())),
            GroupId = req.GroupId,
            GroupName = _narfuApi.Students.GetGroupByRealId(req.GroupId).Name
        }, cancellation: ct);
    }
    
    private static DateTime GetStartOfWeek(DateTime dt)
    {
        const DayOfWeek startOfWeek = DayOfWeek.Monday;
        
        var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}

public class ScheduleRequest
{
    public int GroupId { get; set; }

    [FromQuery]
    public DateTime? Date { get; set; }
}

public class ScheduleResponse
{
    public string GroupName { get; set; }
    public int GroupId { get; set; }
    public Dictionary<DateTime, Dictionary<DateTime, IEnumerable<Lesson>>> Lessons { get; set; }
}