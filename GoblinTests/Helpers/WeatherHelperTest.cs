using Goblin.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoblinTests.Helpers
{
    [TestClass]
    public class WeatherHelperTest
    {
        [TestMethod]
        public void CheckCity_Arh_True()
        {
            var res = WeatherHelper.CheckCity("Архангельск");
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void CheckCity_Unk_False()
        {
            var res = WeatherHelper.CheckCity("фдлытв");
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void GetWeather_Arh_String()
        {
            var res = WeatherHelper.GetWeather("Архангельск");
            Assert.AreNotEqual(string.Empty, res);
        }
    }
}