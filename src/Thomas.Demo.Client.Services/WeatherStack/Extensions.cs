using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thomas.Apis.Core;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.Services.WeatherStack
{
    public static class Extensions
    {
        public static bool IsRaining(this Current weather) => weather.weather_descriptions.Any(x => x.ToLower().Contains("rain"));
        public static async Task<T> GetAsync<T, TError>(this IRestClient client, string resource, Func<TError,string> getErrorMessage, params (string Name, object Value)[] parameters)
        {
            var request = new RestRequest(resource);
            foreach (var parameter in parameters.Where(p=>p.Value != null))
            {
                request.AddParameter(parameter.Name, parameter.Value);
            }
            var responseText = await client.GetAsync<string>(request);
            var errorResponse = JsonConvert.DeserializeObject<TError>(responseText);
            if(getErrorMessage(errorResponse) is string message)
            {
                throw Api.Create.Exception(message);
            }

            var response = JsonConvert.DeserializeObject<T>(responseText);
            return response;
        }
    }
}