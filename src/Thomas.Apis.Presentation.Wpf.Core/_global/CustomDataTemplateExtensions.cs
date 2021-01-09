using System;
using System.Linq;
using System.Windows;
using Thomas.Apis.Core;
using Thomas.Apis.Presentation.Wpf.Core.Attached;

// ReSharper disable once CheckNamespace
internal static class CustomDataTemplateExtensions
{
    //public static Type GetFinalType(this ICustomDataTemplate dataTemplate)
    //{
    //    var dataType = (Type) dataTemplate.DataType;
    //    var finalDataType = dataTemplate.GenericArgument() == null
    //        ? dataType
    //        : dataType.Assembly.GetType(
    //            dataType.FullName + "`1").MakeGenericType(dataTemplate.GenericArgument());
    //    return finalDataType;
    //}

    public static bool Matches(this DataTemplate dataTemplate, Object instance)
    {
        if (instance == null || dataTemplate.FinalType() == null)
        {
            return false;
        }

        var instanceType = instance.GetType();
        bool result;
        if (!dataTemplate.MatchMap().TryGetValue(instanceType, out result))
        {
            result = dataTemplate.FinalType().IsInstanceOfType(instance);
            if (!result && dataTemplate.GenericArgument() != null)
            {
                var softMatch =
                    instanceType.BaseTypes()
                        .Concat(instanceType.GetInterfaces())
                        .Any(
                            t =>
                                t.IsGenericType &&
                                t.GetGenericTypeDefinition() == dataTemplate.FinalType().GetGenericTypeDefinition() &&
                                (t.GetGenericArguments()
                                    .SequenceEqual(new[] {dataTemplate.GenericArgument()},
                                    Api.Create.EqualityComparer<Type>(
                                        (t1, t2) => t1.IsLogicalEqualTo(t2)))));

                result = softMatch;
            }
            dataTemplate.MatchMap().Add(instanceType, result);
        }
        return result;
    }

}
