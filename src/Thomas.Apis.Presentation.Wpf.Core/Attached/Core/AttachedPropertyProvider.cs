using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached.Core
{
    public abstract class AttachedPropertyProvider<TSelf,TOwner>: AttachedPropertyProviderCore<TSelf,TOwner>
        where TOwner: DependencyObject
    {
 
        public static TValue Get<TValue>(TOwner owner, Func<TOwner,TValue> getMethod = null, [CallerMemberName] String methodName = null)
        {
            var dependencyProperty = GetDependencyProperty(methodName);
            return (TValue)owner.GetValue(dependencyProperty);
        }
        public static void Set< TValue>(
           TOwner owner, TValue value, [CallerMemberName] String methodName = null)
        {
            var dependencyProperty = GetDependencyProperty(methodName);
            owner.SetValue(dependencyProperty, value);
        }


    }
}