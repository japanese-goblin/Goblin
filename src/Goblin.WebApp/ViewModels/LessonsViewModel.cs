using System.Collections.Generic;
using Goblin.Narfu.Models;

namespace Goblin.WebApp.ViewModels
{
    public class LessonsViewModel
    {
        public Dictionary<string, Lesson[]> Lessons { get; set; }
        public string GroupTitle { get; set; }
        public string ScheduleLink { get; set; }
        public string WebcalLink { get; set; }
    }
}