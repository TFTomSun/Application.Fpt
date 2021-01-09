using RestSharp;
using System.Linq;
using System.Threading.Tasks;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.Services.WeatherStack
{
    public static class Extensions
    {
        public static bool IsRaining(this Current weather) => weather.weather_descriptions.Any(x => x.ToLower().Contains("rain"));
        public static async Task<T> GetAsync<T>(this IRestClient client, string resource, params (string Name, object Value)[] parameters)
        {
            var request = new RestRequest(resource);
            foreach (var parameter in parameters)
            {
                request.AddParameter(parameter.Name, parameter.Value);
            }
            var response = await client.GetAsync<T>(request);
            return response;
        }
    }
}