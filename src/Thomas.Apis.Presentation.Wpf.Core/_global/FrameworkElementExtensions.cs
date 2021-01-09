using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;


public static class FrameworkElementExtensions
{
    public static BitmapSource ScreenShot(this Visual element, double dpiX = 96.0, double dpiY = 96.0)
    {
        if (element == null)
        {
            return null;
        }
        var bounds = VisualTreeHelper.GetDescendantBounds(element);
        var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
            (int)(bounds.Height * dpiY / 96.0),
            dpiX,
            dpiY,
            PixelFormats.Pbgra32);
        var dv = new DrawingVisual();
        using (var ctx = dv.RenderOpen())
        {
            var vb = new VisualBrush(element);
            ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
        }
        rtb.Render(dv);
        return rtb;
    }

    public static BitmapSource ScreenShot(this Visual element, int pixelWidth, int pixelHeight)
    {
        var renderTargetBitmap =new RenderTargetBitmap(
            pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);

        renderTargetBitmap.Render(element);
        return renderTargetBitmap;
    }

    public static byte[] ToJpegByteArray(this BitmapSource bitmapSource)
    {
        var jpgEncoder = new JpegBitmapEncoder();
        jpgEncoder.QualityLevel = 100;
        return bitmapSource.ToByteArray(jpgEncoder);
    }
    public static byte[] ToPngByteArray(this BitmapSource bitmapSource)
    {
        var encoder = new PngBitmapEncoder();
        return bitmapSource.ToByteArray(encoder);
    }

    public static byte[] ToByteArray(this BitmapSource bitmapSource, BitmapEncoder encoder)
    {

        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        using (var stream = new MemoryStream())
        {
            encoder.Save(stream);
            var data = stream.ToArray();
            return data;
        }
    }


    public static IEnumerable<DependencyObject> ParentDependencyObjects(this FrameworkElement element)
    {
        if (element.Parent != null)
        {
            yield return element.Parent;
        }
        if (element.TemplatedParent != null)
        {
            yield return element.TemplatedParent;
        }
    }

    public static IEnumerable<FrameworkElement> ParentElements(this FrameworkElement element)
    {
        return element.ParentDependencyObjects().OfType<FrameworkElement>();
    }

    public static IEnumerable<FrameworkElement> ParentElements(this FrameworkElement element, bool recursive)
    {
        if (recursive)
        {
            return element.ParentElements().Flatten(p => p.ParentElements());
        }
        return element.ParentElements();
    }

    public static IEnumerable<ResourceDictionary> AllResourceDictionaries(this FrameworkElement element)
    {
        return element.To().Enumerable().Concat(element.ParentElements(true)).Select(
            e => e.Resources).Concat(System.Windows.Application.Current.Resources).SelectMany(
            rd => rd.To().Enumerable().Concat(rd.MergedDictionaries)).Distinct();
    }
}