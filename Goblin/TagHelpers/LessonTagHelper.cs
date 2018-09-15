using Goblin.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
            switch (Lesson.Type)
            {
                case "Лекции":
                    _cssStyle = "green";
                    break;
                case "Лабораторные занятия":
                    _cssStyle = "blue";
                    break;
                case "Практические занятия":
                    _cssStyle = "yellow";
                    break;
            }
        }
    }
}