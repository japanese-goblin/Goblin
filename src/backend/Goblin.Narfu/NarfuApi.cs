using System;
using System.Net.Http;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Schedule;

namespace Goblin.Narfu;

public class NarfuApi : INarfuApi
{
    public ITeacherSchedule Teachers { get; }
    public IStudentsSchedule Students { get; }

    public NarfuApi(string groupsLink, IHttpClientFactory clientFactory)
    {
        var client = clientFactory.CreateClient("narfu-api");
        client.BaseAddress = new Uri("https://ruz.narfu.ru/");
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0");
        client.Timeout = TimeSpan.FromSeconds(5);
        
        Teachers = new TeachersSchedule(client);
        Students = new StudentsSchedule(groupsLink, client);
    }
}