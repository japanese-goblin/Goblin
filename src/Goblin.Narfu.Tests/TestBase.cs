using System;
using System.IO;

namespace Goblin.Narfu.Tests
{
    public class TestBase
    {
        private const string DefaultPath = "TestData";

        protected string StudentsSchedulePath => Path.Combine(DefaultPath, "StudentsSchedule.ics");
        protected string TeachersSchedulePath => Path.Combine(DefaultPath, "TeachersSchedule.html");
        protected string FindByNamePath => Path.Combine(DefaultPath, "FindTeacher.json");

        protected const int CorrectGroup = 271901;
        protected const int IncorrectGroup = 111111;

        protected DateTime CorrectDate = new DateTime(2040, 01, 23, 16, 15, 0);
        protected DateTime IncorrectDate = new DateTime(2010, 01, 23, 16, 15, 0);

        protected const int CorrectTeacherId = 12345;

        protected NarfuApi Api => new NarfuApi();
    }
}