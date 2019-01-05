using System.Collections.Generic;
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

        public static List<User> GetUsers() => Db.Users.ToList();

        public static long[] GetAdmins() => Db.Users.Where(x => x.IsAdmin).Select(x => x.Vk).ToArray();

        public static List<User> GetWeatherUsers() => Db.Users.Where(x => x.City != "" && x.Weather).ToList();

        public static List<User> GetScheduleUsers() => Db.Users.Where(x => x.Group != 0 && x.Schedule).ToList();
    }
}