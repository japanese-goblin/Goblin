using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi
{
    public class GetDailyWeatherAtTests : TestBase
    {
        private readonly DateTime _date = new DateTime(2019, 09, 28);

        [Fact]
        public async Task GetDailyWeatherAt_CorrectCityAndDate_ReturnsModel()
        {
            using var http = new HttpTest();
            http.RespondWith(File.ReadAllText(DailyWeatherPath));
            
            var weather = await Api.GetDailyWeatherAt(CorrectCity, _date);

            weather.Should().NotBeNull();
            weather.Cloudiness.Should().Be(69);
            weather.Humidity.Should().Be(73);
            weather.Pressure.Should().BeApproximately(1018.89, 0.01F);
            weather.Rain.Should().BeNull();
            weather.Snow.Should().BeNull();
            weather.Temperature.Should().NotBeNull();
            weather.Temperature.Day.Should().BeApproximately(10.75F, 0.01F);
            weather.Temperature.Evening.Should().BeApproximately(10.22F, 0.01F);
            weather.Temperature.Max.Should().BeApproximately(11.74F, 0.01F);
            weather.Temperature.Min.Should().BeApproximately(8.16F, 0.01F);
            weather.Temperature.Morning.Should().BeApproximately(10.75F, 0.01F);
            weather.Temperature.Night.Should().BeApproximately(8.16F, 0.01F);
            weather.UnixTime.Should().Be(1569661200);
            weather.Weather.Should().HaveCount(1);
            weather.Weather[0].Icon.Should().Be("04d");
            weather.Weather[0].Id.Should().Be(803);
            weather.Weather[0].Main.Should().Be("Clouds");
            weather.Weather[0].State.Should().Be("пасмурно");
            weather.WindDeg.Should().Be(246);
            weather.WindSpeed.Should().BeApproximately(1.99F, 0.01F);
            weather.ToString().Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetDailyWeatherAt_IncorrectCity_ThrowsException()
        {
            using var http = new HttpTest();
            http.RespondWith(string.Empty, 404);

            Func<Task> func = async () => await Api.GetDailyWeatherAt(IncorrectCity, _date);

            await func.Should().ThrowAsync<FlurlHttpException>();
        }

        [Fact]
        public async Task GetDailyWeatherAt_IncorrectDate_ThrowsException()
        {
            using var http = new HttpTest();
            http.RespondWith(File.ReadAllText(DailyWeatherPath));

            Func<Task> func = async () => await Api.GetDailyWeatherAt(IncorrectCity, _date.AddDays(17));

            await func.Should().ThrowAsync<ArgumentException>();
        }
    }
}