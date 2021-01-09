using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

public static class ImageExtensions
{
    public static BitmapImage ToBitmapImage(this byte[] data)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        var memoryStream = new MemoryStream(data);
        // Save to a memory stream...
        //image.Save(memoryStream, image.RawFormat);
        // Rewind the stream...
        memoryStream.Seek(0, SeekOrigin.Begin);
        bitmap.StreamSource = memoryStream;
        bitmap.EndInit();
        return bitmap;
    }
    public static BitmapImage ToBitmapImage(this Image image)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        var memoryStream = new MemoryStream();
        // Save to a memory stream...
        image.Save(memoryStream, image.RawFormat);
        // Rewind the stream...
        memoryStream.Seek(0, SeekOrigin.Begin);
        bitmap.StreamSource = memoryStream;
        bitmap.EndInit();
        return bitmap;
    }

    public static byte[] ToByteArray(this Image image)
    {
        var memoryStream = new MemoryStream();
        image.Save(memoryStream, image.RawFormat);
        return memoryStream.ToArray();
    }
}