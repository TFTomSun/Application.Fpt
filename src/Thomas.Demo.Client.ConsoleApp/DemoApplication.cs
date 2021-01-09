using CommandLine;
using System;
using System.Threading.Tasks;
using Thomas.Apis.Presentation.CommandLine;
using Thomas.Demo.Client.Services.WeatherStack;

namespace Thomas.Demo.Client.ConsoleApp
{
    /// <summary>
    /// The entry point class for the Demo Weather Command Line app.
    /// </summary>
    class DemoApplication
    {
        public class Options
        {
            /// <summary>
            /// Gets or sets the city name or  zip code.
            /// </summary>
            [Option('l', "location", Required = true, HelpText = "The city name or zip code.")]
            public string Location { get; set; }
        }

        /// <summary>
        /// The entry point method.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns></returns>
        static async Task Main(string[] args) => await new ConsoleApplication().RunAsync<Options>(args,
            async options =>
            {
                var weatherData = await new WeatherStackService().QueryCurrentAsync(options.Location);
                var recommondations = new WeatherRecommendationService().Get(weatherData);
                foreach (var recommondation in recommondations)
                {
                    Console.WriteLine(recommondation.Question);
                    Console.WriteLine(recommondation.Answer ? "yes" : "no");
                }
                return 0;
            });
    }
}
