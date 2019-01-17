using Goblin.Models;
using System.Collections.Generic;
using System.Linq;

namespace Goblin.Helpers
{
    public static class DbHelper
    {
        public static MainContext Db { get; }

        static DbHelper()
        {
            Db = new MainContext();
        }

        public static User[] GetUsers() => Db.Users.ToArray();

        public static long[] GetAdmins() => Db.Users.Where(x => x.IsAdmin).Select(x => x.Vk).ToArray();

        public static User[] GetWeatherUsers() => Db.Users.Where(x => x.City != "" && x.Weather).ToArray();

        public static User[] GetScheduleUsers() => Db.Users.Where(x => x.Group != 0 && x.Schedule).ToArray();
    }
}