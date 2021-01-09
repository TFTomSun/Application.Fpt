using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Thomas.Apis.Core.New;

public static class Extensions
{
    public static Application WithMaterialDesign(this Application app)
    {
        app.Startup += (s, args) =>
        {
            app.Resources.AddMaterialDesign(true);
            app.MainWindow.ApplyMaterialDesign();
        };
        return app;
    }

    private static bool addCache;
    [New("Added default material views")]
    public static ResourceDictionary AddMaterialDesign(this ResourceDictionary resourceDictionary, bool add = true)
    {
        addCache = add;
        if (add)
        {
            var colorTheme = new CustomColorTheme()
            {
                BaseTheme = BaseTheme.Light,
                PrimaryColor = (Color)ColorConverter.ConvertFromString("#01579b"),
                SecondaryColor = Brushes.DarkGreen.Color
            };
            var thisAssembly = typeof(Extensions).Assembly;
            resourceDictionary.MergedDictionaries.Add(colorTheme);
            resourceDictionary.AddXamlResource(@"Views\DefaultViews.xaml", thisAssembly);
            resourceDictionary.AddXamlResource(@"AppStyles\Default.xaml", thisAssembly);
        }
        return resourceDictionary;
    } 

    public static void ApplyMaterialDesign(this Window window)
    {
        if (addCache)
        {
            window.Background = (Brush)window.FindResource("MaterialDesignPaper");
            window.Foreground = (Brush)window.FindResource("MaterialDesignBody");
            window.FontFamily = (FontFamily)window.FindResource("MaterialDesignFont");
        }
    }
}