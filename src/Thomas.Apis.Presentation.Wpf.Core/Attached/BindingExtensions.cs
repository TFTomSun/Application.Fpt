using System;
using System.Windows;
using System.Windows.Data;
using Thomas.Apis.Presentation.Wpf.Core.Attached.Core;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached
{
    public class BindingExtensions : AttachedPropertyProviderCore<BindingExtensions, Binding>
    {
        public static readonly DependencyProperty ConverterTypeProperty = CreateAttached(GetConverterType,onChanged: OnConverterTypeChanged);

        private static void OnConverterTypeChanged(Binding arg1, Type arg2, Type arg3)
        {
        }

        public static Type GetConverterType(Binding binding)
        {
            return binding.Converter?.GetType();
        }

        public static void SetConverterType(Binding binding, Type converterType)
        {
            binding.Converter = (IValueConverter)Activator.CreateInstance(converterType);
            
         }
    }
}
