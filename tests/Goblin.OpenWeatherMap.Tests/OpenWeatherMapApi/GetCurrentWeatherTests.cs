using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi;

public class GetCurrentWeatherTests : TestBase
{
    [Fact]
    public async Task GetCurrentWeather_CorrectCity_ReturnsModel()
    {
        var correctDate = new DateTimeOffset(2019, 09, 28, 09, 17, 58, TimeSpan.Zero);
        var correctSunriseDate = new DateTimeOffset(2019, 09, 28, 03, 25, 23, TimeSpan.Zero);
        var correctSunsetDate = new DateTimeOffset(2019, 09, 28, 15, 15, 14, TimeSpan.Zero);
        using var http = new HttpTest();
        http.RespondWith(await File.ReadAllTextAsync(CurrentWeatherPath));

        var weather = await Api.GetCurrentWeather(CorrectCity);

        weather.Should().NotBeNull();
        weather.Coordinates.Should().NotBeNull();
        weather.Coordinates.Longitude.Should().BeApproximately(37.61F, 0.01F);
        weather.Coordinates.Latitude.Should().BeApproximately(55.75F, 0.01F);
        weather.Info.Should().HaveCount(1);
        weather.Info[0].Icon.Should().Be("03d");
        weather.Info[0].Id.Should().Be(802);
        weather.Info[0].Main.Should().Be("Clouds");
        weather.Info[0].Description.Should().Be("слегка облачно");
        weather.Base.Should().Be("stations");
        weather.Weather.Should().NotBeNull();
        weather.Weather.Humidity.Should().BeApproximately(62F, 0.01F);
        weather.Weather.Pressure.Should().BeApproximately(1018F, 0.01F);
        weather.Weather.Temperature.Should().BeApproximately(10.75F, 0.01F);
        weather.Weather.MinTemp.Should().BeApproximately(10F, 0.01F);
        weather.Weather.MaxTemp.Should().BeApproximately(11.1F, 0.01F);
        weather.Wind.Should().NotBeNull();
        weather.Wind.Degrees.Should().BeApproximately(230F, 0.01F);
        weather.Wind.Speed.Should().BeApproximately(2F, 0.01F);
        weather.Clouds.Should().NotBeNull();
        weather.UnixTime.Should().Be(correctDate);
        weather.OtherInfo.Should().NotBeNull();
        weather.OtherInfo.Country.Should().Be("RU");
        weather.OtherInfo.Id.Should().Be(9029);
        weather.OtherInfo.Message.Should().BeApproximately(0.0094, 0.001F);
        weather.OtherInfo.Sunrise.Should().Be(correctSunriseDate);
        weather.OtherInfo.Sunset.Should().Be(correctSunsetDate);
        weather.OtherInfo.Type.Should().Be(1);
        weather.CityId.Should().Be(524901);
        weather.Visibility.Should().BeApproximately(8000F, 0.01F);
        weather.CityName.Should().Be("Moscow");
        weather.ResponseCode.Should().Be(200);
        weather.ToString().Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetCurrentWeather_IncorrectCity_ThrowsException()
    {
        using var http = new HttpTest();
        http.RespondWith(string.Empty, 404);

        Func<Task> func = async () => await Api.GetCurrentWeather(IncorrectCity);

        await func.Should().ThrowAsync<FlurlHttpException>();
    }
}