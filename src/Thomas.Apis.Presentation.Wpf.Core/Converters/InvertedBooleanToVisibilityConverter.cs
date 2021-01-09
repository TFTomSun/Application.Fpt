using System;
using System.Globalization;
using System.Windows;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    /// <summary>
    /// Convert between boolean and visibility
    /// </summary>
    public class InvertedBooleanToVisibilityConverter : GenericValueConverter< bool, Visibility>
    {
        /// <summary>
        /// Convert bool or Nullable&lt;bool&gt; to Visibility
        /// </summary>
        /// <param name="value">bool or Nullable&lt;bool&gt;</param>
        /// <param name="targetType">Visibility</param>
        /// <param name="parameter">null</param>
        /// <param name="culture">null</param>
        /// <returns>Visible or Collapsed</returns>
        protected override Visibility Convert(bool value, object parameter, CultureInfo culture, Type targetType)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convert Visibility to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected override bool ConvertBack(Visibility value, object parameter, CultureInfo culture, Type targetType)
        {
            return value == Visibility.Collapsed;
        }
    }
}