using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Thomas.Apis.Core;
using Thomas.Apis.Core.Extendable;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.Services.WeatherStack
{
    /// <summary>
    /// The service client implementation for weatherstack.com
    /// </summary>
    public class WeatherStackService
    {
        private IExtendableObject Cache { get; } = new ExtendableObject();
        protected IRestClient Client => this.Cache.Get(
            () => new RestClient("http://api.weatherstack.com/").AddDefaultParameter("access_key",this.AccessKey));

        private string AccessKey { get; }

        /// <summary>
        /// Creates a new instance of the weatherstack.com service.
        /// </summary>
        /// <param name="accessKey">The API key to access the key. Leave it null if you specified the API key throgh the env variable.</param>
        public WeatherStackService(string? accessKey = null)
        {
            AccessKey = (accessKey ?? Environment.GetEnvironmentVariable("WeatherStackApiKey"))
                ?? throw Api.Create.Exception("no api key has been specified. Provide the API key as environment variable 'WeatherStackApiKey' or pass it as parameter to the service.");
        }

        public  Task<ServiceResponse> QueryCurrentAsync(string query)
             => this.Client.GetAsync<ServiceResponse>("current", ("query", query));

    }

    public class WeatherCodeService
    {
        private static IExtendableObject StaticCache { get; } = new ExtendableObject();

        private codes WeatherCodes => StaticCache.Get(() => (codes)new XmlSerializer(typeof(codes)).Deserialize(
            new StringReader(typeof(WeatherCodeService).Assembly.GetResourceFileContent("wwoConditionCodes.xml"))));

        public Task<codesCondition> ResolveCodeAsync(int weatherCode) => Task.FromResult(this.WeatherCodes.condition.Single(
            c => c.code == weatherCode, $"the weather code '{weatherCode}' couldn't be resolved"));
    }

    public class WeatherEvalulationService
    {
        public IEnumerable<(string Question, bool Answer)> Evaluate(ServiceResponse weatherQueryResponse)
        {
            var current = weatherQueryResponse.current;
            yield return ("Should I go out?", !current.IsRaining());
            yield return ("Should I wear sunscreen?", current.uv_index > 3);
            yield return ("Can I fly my kite?", !current.IsRaining() && current.wind_speed > 15);
        }


    }
    

   
}


public static class RestClientExtensions
{
    public static bool IsRaining(this Current weather) => weather.weather_descriptions.Any(x => x.ToLower().Contains("rain"));
   public static async Task<T> GetAsync<T>(this IRestClient client, string resource, params (string Name,object Value)[] parameters)
    {
        var request = new RestRequest(resource);
        foreach(var parameter in parameters)
        {
            request.AddParameter(parameter.Name, parameter.Value);
        }
        var response = await client.GetAsync<T>(request);
        return response;
    }
}