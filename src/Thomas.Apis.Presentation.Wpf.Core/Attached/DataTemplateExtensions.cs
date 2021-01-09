using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Thomas.Apis.Core.Extendable;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached
{
    public static class DataTemplateExtensions //: AttachedPropertyProvider<DataTemplateExtensions, DataTemplate>
    {
        //public static readonly DependencyProperty IsReadOnlyProperty = CreateAttached(GetIsReadOnly,false,OnIsReadOnlyChanged);

        //private static void OnIsReadOnlyChanged(DataTemplate dataTemplate, bool? oldValue, bool? newValue)
        //{
        //}

        public static Dictionary<Type, bool> MatchMap(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get(() => new Dictionary<Type, bool>());
        }

        public static Type FinalType(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get(() =>
            {
                var dataType = (Type) dataTemplate.DataType;

                

                if (dataTemplate.GenericArgument() == null)
                {
                    return dataType;
                }
                if (dataTemplate.GenericArgumentInnerArgument() == null)
                {
                    return dataType.CreateGenericType(dataTemplate.GenericArgument());
                }

                return dataType.CreateGenericType(
                    dataTemplate.GenericArgument().CreateGenericType(
                        dataTemplate.GenericArgumentInnerArgument()));
            });
        }

        private static Type CreateGenericType(this Type baseType, Type genericArgumentType)
        {
            var genericBaseTypeName = baseType.Name + "`1";
            var openGenericType =  baseType.Assembly.GetType(baseType.FullName + "`1") ??
                                   baseType.Assembly.GetTypes().First(t => t.Name == genericBaseTypeName);

            return openGenericType.MakeGenericType(genericArgumentType);
        }


        public static bool IsReadOnly(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<bool>();
        }

        public static void SetIsReadOnly(DataTemplate dataTemplate, bool? value)
        {
            dataTemplate.Set(value);
        }

        public static Type GenericArgument(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<Type>();
        }

        public static Type GenericArgumentInnerArgument(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<Type>();
        }

        public static bool IsHierarchical(this DataTemplate dataTemplate)
        {
            return dataTemplate is HierarchicalDataTemplate;
        }
       

        public static bool IsValueProvider(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<bool>();
        }
        public static void SetIsValueProvider(this DataTemplate dataTemplate, bool value)
        {
            dataTemplate.Set(value);
        }

        public static bool UseImage(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<bool>();
        }
        public static void SetUseImage(this DataTemplate dataTemplate, bool value)
        {
            dataTemplate.Set(value);
        }
        public static int Priority(this DataTemplate dataTemplate)
        {
            return dataTemplate.Get<int>();
        }
        public static void SetPriority(this DataTemplate dataTemplate, int value)
        {
            dataTemplate.Set(value);
        }

        public static void SetGenericArgument(this DataTemplate dataTemplate, Type value)
        {
            dataTemplate.Set(value);
        }

        public static void SetGenericArgumentInnerArgument(this DataTemplate dataTemplate, Type value)
        {
            dataTemplate.Set(value);
        }

        private static ConcurrentDictionary<DataTemplate, IExtendableObject> ExtendableMap { get; } =
            new ConcurrentDictionary<DataTemplate, IExtendableObject>();

        [New("Datatemplate extendable based on a concurrent dictionary")]
        private static IExtendableObject Extendable(this DataTemplate dataTemplate)
        {
          return  ExtendableMap.GetOrAdd(dataTemplate, dt => new ExtendableObject());
        }
        private static T Get<T>(this DataTemplate dataTemplate, Func<T>? createValue = null, [CallerMemberName] string callerName = default!)
        {
            var key = GetKey(callerName);
            return dataTemplate.Extendable().Get(createValue,key);
        }

        private static void Set<T>(this DataTemplate dataTemplate, T value, [CallerMemberName] string callerName = default!)
        {
           
            var key = GetKey(callerName);
            dataTemplate.Extendable().Set( value, key);
        }

        private static string GetKey(string callerName)
        {
            var key = callerName;
            if (key.StartsWith("Set") || key.StartsWith("Get"))
            {
                key = key.Substring(3);
            }
            return key;
        }

        //public static IExtendableObject  Extendable(this ResourceDictionary dictionary)
        //{
        //    var extendable = dictionary[nameof(Extendable)] as IExtendableObject;
        //    if (extendable == null)
        //    {
        //        extendable = new ExtendableObject();
        //        dictionary[nameof(Extendable)] = extendable;
        //    }
        //    return extendable;
        //}
    
    }
}