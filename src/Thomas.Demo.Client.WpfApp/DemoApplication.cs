using System;
using System.Windows;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.ViewModels;

namespace Thomas.Demo.Client.WpfApp
{
    class DemoApplication
    {
        [STAThread]
        static int Main(string[] args)  => 
            new Application().WithMaterialDesign().Run<MainViewModel>(
                new Size(450,300), typeof(WeatherStackService).Assembly);
    }
}
