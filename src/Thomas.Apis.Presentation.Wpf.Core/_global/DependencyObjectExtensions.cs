
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using Thomas.Apis.Core;

public static class DependencyObjectExtensions
{
    public static Binding Bind(this DependencyObject target, DependencyProperty targetProperty,
        DependencyObject source, DependencyProperty sourceProperty,
        BindingMode mode = BindingMode.TwoWay)
    {
        var binding = new Binding();
        binding.Source = source;
        binding.Path = new PropertyPath(sourceProperty.Name);
        binding.Mode = mode;
        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        binding.NotifyOnSourceUpdated = true;
        binding.NotifyOnTargetUpdated = true;
        //        
        BindingOperations.SetBinding(target, targetProperty, binding);
        return binding;
    }


    public static TValue Get<TSource, TValue>(this TSource dependencyObject, Func<TSource, TValue> propertyExpression, [CallerMemberName] string? propertyName = null)
        where TSource : DependencyObject
    {
        var dependencyPropertyInfo = dependencyObject.GetDependencyProperty(propertyName);
        return (TValue) dependencyObject.GetValue(dependencyPropertyInfo.Item2);
    }

    public static void Set<TSource, TValue>(this TSource dependencyObject, Func<TSource, TValue> propertyExpression,
        TValue newValue, PropertyChangedEventHandler propertyChangedEventHandler = null, [CallerMemberName] string? propertyName = null)
        where TSource : DependencyObject
    {
        var dependencyPropertyInfo = dependencyObject.GetDependencyProperty(propertyName);
        dependencyObject.SetValue(dependencyPropertyInfo.Property, newValue);

        propertyChangedEventHandler?.Invoke(dependencyObject,
            new PropertyChangedEventArgs(dependencyPropertyInfo.PropertyName));
    }

    internal static (string PropertyName, DependencyProperty Property) GetDependencyProperty<TSource>(
        this TSource dependencyObject, string propertyName)
        where TSource : DependencyObject
    {
        if (dependencyObject == null) throw new ArgumentNullException(nameof(dependencyObject));
        return (propertyName,dependencyObject.GetDependencyProperty(propertyName,true));
    }
    internal static DependencyProperty  GetDependencyProperty<TSource>(
      this TSource dependencyPropertyProvider, String propertyName, bool throwException= true)
    {
        //if (dependencyPropertyProvider == null) throw new ArgumentNullException(nameof(dependencyPropertyProvider));
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

        var dependencyPropertyName = propertyName + "Property";
        var type = dependencyPropertyProvider?.GetType() ?? typeof(TSource);
        var types = type.To().Enumerable().Concat(type.BaseTypes());
        var dependencyPropertyField = types.Select(t=>t.GetField(dependencyPropertyName, BindingFlags.Static| BindingFlags.Public)).FirstOrDefault(x=>x!=null);
        if (dependencyPropertyField == null && throwException)
        {
            throw Api.Create.Exception(
                $"The dependency property '{propertyName}' doesn't exist on the source of type '{typeof(TSource).FullName}'.");
        }
        var dependencyProperty = (DependencyProperty)dependencyPropertyField?.GetValue(dependencyPropertyProvider);
        return dependencyProperty;
    }

    public static bool IsValid(this DependencyObject dependencyObject)
    {
        // The dependency object is valid if it has no errors and all
        // of its children (that are dependency objects) are error-free.
        if (dependencyObject == null)
            return false;
        return !Validation.GetHasError(dependencyObject) && LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>().All(IsValid);
    }


}