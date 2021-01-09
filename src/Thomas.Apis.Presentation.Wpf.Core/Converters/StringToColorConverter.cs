using System;
using System.Globalization;
using System.Windows.Media;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    [New]
    public class StringToColorConverter : GenericValueConverter< string, Color>
    {
        protected override Color Convert(string value, object parameter, CultureInfo culture, Type targetType)
        {
            return (Color)ColorConverter.ConvertFromString(value);
        }

        protected override string ConvertBack(Color value, object parameter, CultureInfo culture, Type targetType)
        {
            return new ColorConverter().ConvertToString(value);
        }
    }
}