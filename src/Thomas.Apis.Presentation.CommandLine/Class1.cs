using CommandLine;
using System;
using System.Threading.Tasks;

namespace Thomas.Apis.Presentation.CommandLine
{
    public class ConsoleApplication
    {
        public async Task<int> RunAsync<T>(string[]? args, Func<T,Task<int>> asyncAppRun, bool askForRerun = true)
        {
            var options = ParseArguments<T>(args);
            var exitCode = await asyncAppRun(options);

            if (askForRerun)
            {
                Console.WriteLine("Would you like to run the program again? (y/n)");
                if (Console.ReadLine() == "y")
                {
                    return await RunAsync(null, asyncAppRun);
                }
            }
            return exitCode;
        }


        private T ParseArguments<T>(string[]? args)
        {
            if ((args?.Length ?? 0) == 0)
            {
                args = new[] { "--help" };
            }
            var result = Parser.Default.ParseArguments<T>(args)
                .MapResult(o => o, e =>
                {
                    Console.WriteLine("Please enter valid arguments:");
                    var newArgs = Console.ReadLine();
                    return ParseArguments<T>(newArgs.Split(' '));
                });
            return result;
        }

    }
}
