using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;

namespace Goblin.Narfu.Tests;

public class TestBase
{
    private const string DefaultPath = "TestData";

    protected const int CorrectGroup = 271901;
    protected const int IncorrectGroup = 111111;

    protected const int CorrectTeacherId = 12345;

    protected const string TeacherName = "Абрамова";

    protected string StudentsSchedulePath => Path.Combine(DefaultPath, "StudentsSchedule.ics");
    protected string TeachersSchedulePath => Path.Combine(DefaultPath, "TeachersSchedule.html");
    protected string FindByNamePath => Path.Combine(DefaultPath, "FindTeacher.json");

    protected INarfuApi Api { get; }

    protected DateTime CorrectDate = new DateTime(2040, 01, 23, 16, 15, 0);
    protected DateTime IncorrectDate = new DateTime(2010, 01, 23, 16, 15, 0);

    public TestBase()
    {
        var mockHttp = new MockHttpMessageHandler();
        
        var res = File.ReadAllText(StudentsSchedulePath);
        mockHttp.When("*")
                .WithQueryString("cod", CorrectGroup.ToString())
                .Respond(MediaTypeNames.Text.Html, res);
        
        mockHttp.When("*")
                .WithQueryString("cod", IncorrectGroup.ToString())
                .Respond(MediaTypeNames.Text.Html, res);
        mockHttp.When("*")
                .WithQueryString("term", TeacherName)
                .Respond(MediaTypeNames.Application.Json, File.ReadAllText(FindByNamePath));
        
        mockHttp.When("*")
                .WithQueryString("lecturer", CorrectTeacherId.ToString())
                .Respond(MediaTypeNames.Text.Html, File.ReadAllText(TeachersSchedulePath));
        
        mockHttp.When("http://groups/").Respond("application/json", @"[
        {
            ""RealId"": 271901,
            ""SiteId"": 14068,
            ""Name"": ""Строительство (Строительство)""
        }]");

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(p => p.CreateClient(It.IsAny<string>()))
               .Returns(mockHttp.ToHttpClient());

        Api = new NarfuApi("http://groups/", factory.Object,
                           Mock.Of<ILogger<Schedule.TeachersSchedule>>(), Mock.Of<ILogger<Schedule.StudentsSchedule>>());
    }
}