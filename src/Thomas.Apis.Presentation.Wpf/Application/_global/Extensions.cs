using Autofac;
using System.Reflection;
using System.Windows;
using Thomas.Apis.Presentation.Wpf;
using Thomas.Apis.Presentation.Wpf.Application;

public static class Extensions
{
    public static Application WithCustomMainWindow(this Application app, Window window)
    {
        app.MainWindow = window;
        return app;
    }

    public static int Run<TModel>(this Application app, Size? defaultWindowSize, params Assembly[] serviceAssemblies)
    {
        var window = app.MainWindow ?? new DefaultMainWindow();
        if(defaultWindowSize is Size size)
        {
            window.Width = size.Width;
            window.Height = size.Height;
        }
        var appContainer = new WpfAppBootstrap().Build<TModel>(window, serviceAssemblies);
        window.DataContext = appContainer.Resolve<TModel>();
        app.Resources.AddDefaultViews();
        return app.Run(window);
    }



}