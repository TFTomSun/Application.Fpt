using System;
using System.Globalization;
using System.Windows;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    public class BoolToVisibilityConverter : GenericValueConverter<bool, Visibility>
    {
        public bool IsNegated { get; }

        public Visibility InvisibleState
        {
            get;
        }

        protected BoolToVisibilityConverter(bool negated, Visibility invisibleState )
        {
            this.IsNegated = negated;
            this.InvisibleState = invisibleState;
        }

        public BoolToVisibilityConverter() : this(false, Visibility.Collapsed)
        {
            
        }

        protected override Visibility Convert(bool value, object parameter, CultureInfo culture, Type targetType)
        {
            var negate = this.NegateRequested(parameter); 
            var result =  value ^ negate ? Visibility.Visible :this.GetInvisibleState(parameter);
            return result;
        }

        private Visibility GetInvisibleState(object parameter)
        {
            return (parameter as string)?.ToEnum<Visibility>() ?? this.InvisibleState;
        }

        private bool NegateRequested(object parameter)
        {
            var parameterString = parameter?.ToString() ?? String.Empty;
            var negate = parameterString.TryToBoolean() ?? this.IsNegated;
            return negate;
        }

        protected override bool ConvertBack(Visibility value, object parameter, CultureInfo culture, Type targetType)
        {
            var negate = this.NegateRequested(parameter);
            var result = negate ^ value == Visibility.Visible;
            return result;
        }
        
    }
}