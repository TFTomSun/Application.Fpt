using System;
using System.Windows;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.ViewModels;

namespace Thomas.Demo.Client.WpfApp
{
    /// <summary>
    /// The entry point class.
    /// </summary>
    class DemoApplication
    {
        /// <summary>
        /// The entry point for the WPF Weather Demo App.
        /// </summary>
        /// <returns>Always zero</returns>
        [STAThread]
        static int Main()  => 
            new Application().WithMaterialDesign().Run<MainViewModel>(
                new Size(450,300), typeof(WeatherStackService).Assembly);
    }
}
