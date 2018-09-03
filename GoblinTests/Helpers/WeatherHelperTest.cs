using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class WeatherHelperTest
    {
        [TestMethod]
        public async Task CheckCity_Arh_True()
        {
            var res = await WeatherHelper.CheckCity("Архангельск");
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public async Task CheckCity_Unk_False()
        {
            var res = await WeatherHelper.CheckCity("фдлытв");
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public async Task GetWeather_Arh_String()
        {
            var res = await WeatherHelper.GetWeather("Архангельск");
            Assert.AreNotEqual(string.Empty, res);
        }
    }
}