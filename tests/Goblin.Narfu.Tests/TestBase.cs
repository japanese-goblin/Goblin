using System;
using System.IO;
using Flurl.Http.Testing;
using Goblin.Narfu.Abstractions;

namespace Goblin.Narfu.Tests
{
    public class TestBase
    {
        private const string DefaultPath = "TestData";

        protected const int CorrectGroup = 271901;
        protected const int IncorrectGroup = 111111;

        protected const int CorrectTeacherId = 12345;

        protected string StudentsSchedulePath => Path.Combine(DefaultPath, "StudentsSchedule.ics");
        protected string TeachersSchedulePath => Path.Combine(DefaultPath, "TeachersSchedule.html");
        protected string FindByNamePath => Path.Combine(DefaultPath, "FindTeacher.json");

        protected INarfuApi Api { get; }

        protected DateTime CorrectDate = new DateTime(2040, 01, 23, 16, 15, 0);
        protected DateTime IncorrectDate = new DateTime(2010, 01, 23, 16, 15, 0);

        public TestBase()
        {
            using var http = new HttpTest();
            {
                http.RespondWith(File.ReadAllText("Data/Groups.json"));
                Api = new NarfuApi("https://1.1.1.1/");
            }
        }
    }
}