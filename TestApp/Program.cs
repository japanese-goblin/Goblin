using Narfu.Parsers;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = SchoolParser.GetSchools().GetAwaiter().GetResult();
            var group = GroupsParser.GetGroupsFromSchool(x[0]).GetAwaiter().GetResult();
        }
    }
}
