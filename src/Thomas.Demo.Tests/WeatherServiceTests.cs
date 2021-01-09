using NUnit.Framework;
using System.Threading.Tasks;
using Thomas.Demo.Client.Services.WeatherStack;

namespace Thomas.Demo.Tests
{
    internal class WeatherServiceTests
    {
        [Test]
        public async Task ResolveWeatherCodeTest()
        {
            var weatherCodeService = new WeatherCodeService();
            var response = await weatherCodeService.ResolveCodeAsync(389);
            Assert.AreEqual("wsymbol_0024_thunderstorms", response.day_icon);
        }

        [Test]
        public async Task QueryAsyncTest()
        {
            var weatherService = new WeatherStackService();
            var response = await weatherService.QueryCurrentAsync("New York");
            Assert.Greater(response.current.pressure, 0);        
        }
     
    }
}
