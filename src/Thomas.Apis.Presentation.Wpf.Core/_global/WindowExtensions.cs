using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

public static class WindowExtensions
{
    public static IEnumerable<DependencyObject> LogicalChildren(this DependencyObject depObj) 
    {
        if (depObj == null)
        {
            return Enumerable.Empty<DependencyObject>();
        }
        return LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>();
    }

    public static IEnumerable<DependencyObject> VisualChildren(this DependencyObject depObj)
    {
        if (depObj != null)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                yield return child;
            }
        }
    }

    public static IntPtr? Handle(this Visual visual)
    {
        return (PresentationSource.FromVisual(visual) as HwndSource)?.Handle;
    }
    public static IntPtr Handle(this Window window)
    {
        return new WindowInteropHelper(window).Handle;
    }
    public static Window Window(this DependencyObject element)
    {
        return System.Windows.Window.GetWindow(element);
    }

    public static bool IsWindowVisible(this Window window)
    {
        var win = new WindowInteropHelper(window);
        var x = (int) (window.Left + window.Width/2);
        var y = (int) (window.Top + window.Height/2);
        var p = new Point(x, y);
        return win.Handle == NativeMethods.WindowFromPoint(p);
    }
}