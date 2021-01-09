using System.Windows;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached
{
    public class ResourceTemplateSelectorExtensions : DependencyObject
    {
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached("IsReadOnly", typeof (bool?), typeof (ResourceTemplateSelectorExtensions));
        public static readonly DependencyProperty UseImageProperty =
            DependencyProperty.RegisterAttached("UseImage", typeof(bool?), typeof(ResourceTemplateSelectorExtensions));

        public static bool? GetIsReadOnly(DependencyObject obj)
        {
            return (bool?) obj.GetValue(IsReadOnlyProperty);
        }

        public static void SetIsReadOnly(DependencyObject obj, bool? value)
        {
            obj.SetValue(IsReadOnlyProperty, value);
        }

        public static bool? GetUseImage(DependencyObject obj)
        {
            return (bool?) obj.GetValue(UseImageProperty);
        }

        public static void SetUseImage(DependencyObject obj, bool? value)
        {
            obj.SetValue(UseImageProperty, value);
        }
    }
}