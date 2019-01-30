using System.Linq;
using Goblin.Models;

namespace Goblin.Helpers
{
    public static class DbHelper
    {
        public static MainContext Db { get; }

        static DbHelper()
        {
            Db = new MainContext();
        }

        public static User[] GetUsers()
        {
            return Db.Users.ToArray();
        }

        public static long[] GetAdmins()
        {
            return Db.Users.Where(x => x.IsAdmin).Select(x => x.Vk).ToArray();
        }

        public static User[] GetWeatherUsers()
        {
            return Db.Users.Where(x => x.City != "" && x.Weather).ToArray();
        }

        public static User[] GetScheduleUsers()
        {
            return Db.Users.Where(x => x.Group != 0 && x.Schedule).ToArray();
        }
    }
}
