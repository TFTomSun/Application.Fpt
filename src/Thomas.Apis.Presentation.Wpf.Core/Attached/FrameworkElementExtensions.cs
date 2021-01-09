using System.Windows;
using System.Windows.Input;
using Thomas.Apis.Presentation.Wpf.Core.Attached.Core;

namespace Thomas.Apis.Presentation.Wpf.Core.Attached
{
    public class FrameworkElementExtensions : AttachedPropertyProvider<FrameworkElementExtensions,FrameworkElement>
    {

        public static readonly DependencyProperty IsAccessKeyScopeProperty = CreateAttached(GetIsAccessKeyScope,false, OnIsAccessKeyScopeChanged);

        private static void OnIsAccessKeyScopeChanged(FrameworkElement element, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                AccessKeyManager.AddAccessKeyPressedHandler(element, HandleScopedElementAccessKeyPressed);
            }
            else
            {
                AccessKeyManager.RemoveAccessKeyPressedHandler(element, HandleScopedElementAccessKeyPressed);
            }
        }

        private static void HandleScopedElementAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) && !Keyboard.IsKeyDown(System.Windows.Input.Key.RightAlt) && GetIsAccessKeyScope((FrameworkElement)sender))
            {
                e.Scope = sender;
                e.Handled = true;
            }
        }

        public static bool GetIsAccessKeyScope(FrameworkElement element)
        {
            return Get(element, GetIsAccessKeyScope);
        }

        public static void SetIsAccessKeyScope(FrameworkElement element, bool value)
        {
            Set(element, value);
        }

        //public static readonly DependencyProperty ContextMenuModelProperty = CreateAttached(GetContextMenuModel, onChanged: ContextMenuDataChanged);

        //private static void ContextMenuDataChanged(FrameworkElement frameworkElement, ContextMenuViewModel oldValue, ContextMenuViewModel newValue)
        //{
        //    if (newValue == null)
        //    {
        //        frameworkElement.ContextMenu = null;
        //    }
        //    else
        //    {
        //        var contextMenu = new ContextMenu();
        //        contextMenu.DataContext = newValue;
        //        contextMenu.SetBinding(ContextMenu.ItemsSourceProperty, nameof(ContextMenuViewModel.Items));
        //        contextMenu.SetBinding(ContextMenu.IsOpenProperty,
        //            new Binding(nameof(ContextMenuViewModel.IsOpen)) { Mode = System.Windows.Data.BindingMode.OneWayToSource });
        //        contextMenu.SetBinding(ContextMenu.VisibilityProperty,
        //            new Binding(nameof(ContextMenuViewModel.IsVisible)) { Converter = new BooleanToVisibilityConverter() });

        //        frameworkElement.ContextMenu = contextMenu;
        //    }

        //}

        //public static ContextMenuViewModel GetContextMenuModel(FrameworkElement control)
        //{
        //    return Get(control, GetContextMenuModel);
        //}
        //public static void SetContextMenuModel(FrameworkElement control, ContextMenuViewModel value)
        //{
        //    Set(control, value);
        //}



        public static readonly DependencyProperty ProvidesCustomDataTemplatesProperty = CreateAttached(GetProvidesCustomDataTemplates);

        public static bool GetProvidesCustomDataTemplates(FrameworkElement element)
        {
            return Get(element, GetProvidesCustomDataTemplates);
        }

        public static void SetProvidesCustomDataTemplates(FrameworkElement element, bool value)
        {
            Set(element, value);
        }
    }
}