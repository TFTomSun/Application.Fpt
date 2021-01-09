using RestSharp;
using System;
using System.Text;
using System.Threading.Tasks;
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

        public  Task<WeatherQueryResponse> QueryCurrentAsync(string query)
             => this.Client.GetAsync<WeatherQueryResponse>("current", ("query", query));

    }
    

   
}
