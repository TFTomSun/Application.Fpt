using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Thomas.Apis.Core.New;

// ReSharper disable once CheckNamespace
public static class ResourceDictionaryExtensions

{
    public static T[] ResourceValues<T>(this ResourceDictionary dictionary)
    {
        var dictionaryValues = dictionary.Values.OfType<T>();
        var mergedDictionaryValues = dictionary.MergedDictionaries.SelectMany(md => md.Values.OfType<T>());
        return dictionaryValues.Concat(mergedDictionaryValues).ToArray();
    }

    public static DependencyObject VisualParent(this DependencyObject dependencyObject)
    {
        return VisualTreeHelper.GetParent(dependencyObject);
    }

    public static IEnumerable<DependencyObject> VisualParents(this DependencyObject dependencyObject)
    {
        var parent = dependencyObject.VisualParent();
        while (parent != null)
        {
            yield return parent;
            parent = parent.VisualParent();
        }
    }

    public static void AddXamlResource(this ResourceDictionary dictionary, string xamlFileName, Assembly xamlFileAssembly)
    {
        dictionary.AddXamlResource(xamlFileName, xamlFileAssembly.GetName().Name);

    }
    public static void AddXamlResource(this ResourceDictionary dictionary, string xamlFileName, string xamlFileAssemblyName)
    {
        dictionary.MergedDictionaries.Add(CreateResourceDictionaryFromFile(xamlFileName,  xamlFileAssemblyName));
    }

    [New("Removed data template processing")]
    private static ResourceDictionary CreateResourceDictionaryFromFile(string xamlFileName,string assemblyName)
    {
        var dictionary = new ResourceDictionary
        {
            Source = CreateUri(xamlFileName, assemblyName)
        };
        //if (processDataTemplates)
        //{

        //    dictionary.ResourceValues<DataTemplate>().ForEach(dt => dt.Resources.Extendable());
        //}
        return dictionary;
    }

    private static Uri CreateUri(String xamlFileName, string assemblyName)
    {
        return new Uri(
            $"pack://application:,,,/{assemblyName};component/{xamlFileName}");
    }
}