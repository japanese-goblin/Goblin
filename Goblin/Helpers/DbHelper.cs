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

        public static List<User> GetUsers()
        {
            return Db.Users.ToList();
        }

        public static List<User> GetWeatherUsers()
        {
            return Db.Users.Where(x => x.City != "" && x.Weather).ToList();
        }

        public static List<User> GetScheduleUsers()
        {
            return Db.Users.Where(x => x.Group != 0 && x.Schedule).ToList();
        }
    }
}