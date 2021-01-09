using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    public class EnumToEnumValueConverter : GenericValueConverter<Enum,Enum>
    {
        protected override Enum Convert(Enum value, object parameter, CultureInfo culture, Type targetType)
        {
            return Convert(value, targetType);
        }

        private static Enum Convert(Enum value, Type targetType)
        {
            return (Enum)Enum.Parse(targetType,
                value.GetAttributes<DescriptionAttribute>().SingleOrDefault()?.Description ?? value.ToString());
        }

        protected override Enum ConvertBack(Enum value, object parameter, CultureInfo culture, Type targetType)
        {
            return Convert(value, targetType);
        }
    }
}