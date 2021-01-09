using System.Windows;

public static class ViewsExtensions
{
    public static ResourceDictionary AddDefaultViews(this ResourceDictionary resources)
    {

        resources.AddXamlResource("Views/DefaultViews.xaml", 
            typeof(ViewsExtensions).Assembly);
        return resources;
    }
}

