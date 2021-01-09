
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using Thomas.Apis.Presentation.Wpf.Core;

public static class MetaDataExtensions
{
    public static DependencyProperty AttachedProperty<TSource, TOwner, TValue>(
        this IMetaData<TSource> factory, Func<TOwner, TValue> getMethod,TValue defaultValue = default(TValue), Action<TOwner,TValue,TValue> onChanged = null)
    {
        var attachedPropertyName = getMethod.Method.Name.Substring(3);


        var metadata = new FrameworkPropertyMetadata();
        metadata.DefaultValue = defaultValue;
        metadata.PropertyChangedCallback = onChanged == null ? null : new PropertyChangedCallback(
            (s,args)=>onChanged((TOwner)(Object)s,(TValue)args.OldValue,(TValue)args.NewValue));
        
        var attachedProperty = System.Windows.DependencyProperty.RegisterAttached(
            attachedPropertyName, typeof(TValue), typeof(TSource), metadata);
    
        return attachedProperty;
    }

    public static DependencyProperty DependencyProperty<TSource, TValue>(
        this IMetaData<TSource> factory, 
        Expression<Func<TSource, TValue>> instancePropertyExpression,
        Action<TSource, TValue, TValue>? onValueChanged = null, TValue defaultValue = default(TValue),
        FrameworkPropertyMetadataOptions options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, [CallerMemberName] string fieldName = default!)
    {
        PropertyMetadata metadata;
        if (onValueChanged == null)
        {
            metadata = new FrameworkPropertyMetadata(defaultValue, options);
        }
        else
        {
            metadata = new FrameworkPropertyMetadata(
                defaultValue, options, (o, args) => onValueChanged(
                    (TSource) (object) o, (TValue) args.OldValue, (TValue) args.NewValue));
        }

        var instancePropertyName = fieldName.Substring(0,fieldName.Length-"Property".Length);

        var dependencyProperty = System.Windows.DependencyProperty.Register(
            instancePropertyName, typeof (TValue), typeof (TSource), metadata);
        
        return dependencyProperty;
    }
}