using Microsoft.AspNetCore.Razor.TagHelpers;
using Goblin.Schedule.Models;

namespace Goblin.TagHelpers
{
    public class LessonTagHelper : TagHelper
    {
        public Lesson Lesson { get; set; }
        private string _cssStyle;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            GetStyleForLessonType();
            output.Content.SetHtmlContent(
                $@"<div class='timetable_sheet {_cssStyle}'>
	                <span class='time_para'>{Lesson.StartEndTime}</span>
	                <span class='kindOfWork'>{Lesson.Type}</span>
	                <span class='discipline'>{Lesson.Name} ({Lesson.Teacher})</span>
	                <span class='group'>{Lesson.Groups}</span>
	                <span class='auditorium'><b>ауд. {Lesson.Auditory} </b> {Lesson.Address}</span>
                </div>");
        }

        private void GetStyleForLessonType()
        {
            if (Lesson.Type == "Лекции")
                _cssStyle = "green";
            else if (Lesson.Type == "Лабораторные занятия")
                _cssStyle = "blue";
            else if (Lesson.Type == "Практические занятия")
                _cssStyle = "yellow";
            else if (Lesson.Type == "Консультация")
                _cssStyle = "orange";
            else if (Lesson.Type.ToLower().Contains("экзамен") || Lesson.Type.ToLower().Contains("зачет")) _cssStyle = "red";
        }
    }
}