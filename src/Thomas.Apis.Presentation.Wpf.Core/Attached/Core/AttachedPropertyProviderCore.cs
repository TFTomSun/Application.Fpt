using System;
using System.Collections.Generic;
using System.Windows;
using Thomas.Apis.Core;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached.Core
{
    public abstract class AttachedPropertyProviderCore<TSelf, TOwner>
    {
        protected static readonly IMetaData<TSelf> MetaData = Api.Global.MetaDataOf<TSelf>();

        protected static DependencyProperty CreateAttached<TValue>(
            Func<TOwner, TValue> getMethod, TValue defaultValue = default(TValue), Action<TOwner, TValue, TValue> onChanged = null)
        {
            return MetaData.AttachedProperty(getMethod, defaultValue, onChanged);
        }

        private static readonly Dictionary<String, DependencyProperty> dependencyPropertyMap =
            new Dictionary<string, DependencyProperty>();

        protected static DependencyProperty GetDependencyProperty(String methodName)
        {
            DependencyProperty property;
            if (!dependencyPropertyMap.TryGetValue(methodName, out property))
            {
                property = default(TSelf).GetDependencyProperty(methodName.Substring(3));
                dependencyPropertyMap.Add(methodName, property);
            }
            return property;
        }
    }
}