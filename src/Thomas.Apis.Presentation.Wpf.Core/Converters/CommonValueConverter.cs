using System;
using System.Globalization;
using Thomas.Apis.Core.Extendable;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{


    /// <summary>
    /// A base class for value converters
    /// </summary>
    /// <typeparam name="TTFrom">The type of the source values.</typeparam>
    /// <typeparam name="TTo">The type of the target values.</typeparam>
    public abstract class CommonValueConverter<TTFrom, TTo> : ExtendableObject
    {
        /// <summary>
        /// Convert the value
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The type of the target value.</param>
        /// <param name="parameter">An additional parameter that can be used for the value conversion</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>The converted value.</returns>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = this.Convert((TTFrom)value, parameter, culture, targetType);
            return result;
        }

        /// <summary>
        /// Converts the target value back to the source value.
        /// </summary>
        /// <param name="value">The target value</param>
        /// <param name="targetType">The type of the source value.</param>
        /// <param name="parameter">An additional parameter that can be used for the value conversion</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>The converted value.</returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = this.ConvertBack((TTo)value, parameter, culture, targetType);
            return result;
        }
        /// <summary>
        /// Convert the value
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">The type of the target value.</param>
        /// <param name="parameter">An additional parameter that can be used for the value conversion</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>The converted value.</returns>
        protected abstract TTo Convert(TTFrom value, object parameter, CultureInfo culture, Type targetType);

        /// <summary>
        /// Converts the target value back to the source value.
        /// </summary>
        /// <param name="value">The target value</param>
        /// <param name="targetType">The type of the source value.</param>
        /// <param name="parameter">An additional parameter that can be used for the value conversion</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>The converted value.</returns>
        protected abstract TTFrom ConvertBack(TTo value, object parameter, CultureInfo culture, Type targetType);


    }
}