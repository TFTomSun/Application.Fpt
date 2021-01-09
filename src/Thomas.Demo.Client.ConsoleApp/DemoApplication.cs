using CommandLine;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.ViewModels;

namespace Thomas.Demo.Client.ConsoleApp
{
    class DemoApplication
    {
        public class Options
        {
            [Option('l', "location", Required = true, HelpText = "The city name or zip code.")]
            public string Location { get; set; }
        }


        static Options ParseArguments(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .MapResult(o => o, e =>
                {
                    Console.WriteLine("Please enter valid arguments:");
                    var newArgs = Console.ReadLine();
                    return ParseArguments(newArgs.Split(' '));
                });
            return result;
        }
        static async Task Main(string[] args) => await RunAsync(args);

        static async Task RunAsync(params string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { "--help" };
            }
            var options = ParseArguments(args);
            var weatherData = await new WeatherStackService().QueryCurrentAsync(options.Location);
            var recommondations = new WeatherEvalulationService().Evaluate(weatherData);
            foreach (var recommondation in recommondations)
            {
                Console.WriteLine(recommondation.Question);
                Console.WriteLine(recommondation.Answer ? "yes" : "no");
            }

            Console.WriteLine("Would you like to run the program again? (y/n)");
            if (Console.ReadLine() == "y")
            {
                await RunAsync();
            }
        }
       
    }
}
