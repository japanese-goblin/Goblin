using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule
{
    public class FindByNameTests : TestBase
    {
        [Fact]
        public async Task FindByName_CorrectName_ReturnsTeachers()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(FindByNamePath));

                var teachers = await Api.Teachers.FindByName("Абрамова");
                var first = teachers.First();
                
                Assert.NotEmpty(teachers);
                Assert.Equal("Кафедра информационных систем и технологий", first.Depart);
                Assert.Equal(31257, first.Id);
                Assert.Equal("Абрамова Любовь Валерьевна", first.Name);
            }
        }
    }
}