using CommandLine;
using System;
using System.Threading.Tasks;
using Thomas.Apis.Presentation.CommandLine;
using Thomas.Demo.Client.Services.WeatherStack;

namespace Thomas.Demo.Client.ConsoleApp
{
    class DemoApplication
    {
        public class Options
        {
            [Option('l', "location", Required = true, HelpText = "The city name or zip code.")]
            public string Location { get; set; }
        }

        static async Task Main(string[] args) => await new ConsoleApplication().RunAsync<Options>(args,
            async options =>
            {
                var weatherData = await new WeatherStackService().QueryCurrentAsync(options.Location);
                var recommondations = new WeatherEvalulationService().Evaluate(weatherData);
                foreach (var recommondation in recommondations)
                {
                    Console.WriteLine(recommondation.Question);
                    Console.WriteLine(recommondation.Answer ? "yes" : "no");
                }
                return 0;
            });
    }
}
