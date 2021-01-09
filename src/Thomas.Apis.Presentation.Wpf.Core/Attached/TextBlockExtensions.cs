using System;
using System.Windows;
using System.Windows.Controls;
using Thomas.Apis.Presentation.Wpf.Core.Attached.Core;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached
{
    public class TextBlockExtensions : AttachedPropertyProvider<TextBlockExtensions,TextBlock>
    {
      

       


        public static readonly DependencyProperty AutoShrinkProperty =
          CreateAttached(GetAutoShrink, false, OnAutoShrinkChanged);


        

        private static void OnAutoShrinkChanged(TextBlock textBlock, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                var style =  new Style();
                var trigger = new Trigger {Property = TextBlock.TextProperty, Value = String.Empty};
                trigger.Setters.Add(new Setter {Property = TextBlock.VisibilityProperty, Value = Visibility.Collapsed});
                style.Triggers.Add(trigger);
                style.Setters.Add(new Setter {Property = TextBlock.VisibilityProperty, Value = Visibility.Visible});
                textBlock.Style = style;
            }
        }

        public static bool GetAutoShrink(TextBlock target)
        {
            return Get(target, GetAutoShrink);
        }
        public static void SetAutoShrink(TextBlock target, bool value)
        {
            Set(target, value);
        }
    }


}
