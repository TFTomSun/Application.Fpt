using System;
using System.Globalization;
using System.Windows.Controls;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    internal class CustomValidationRule<TView> : ValidationRule
    {
        private readonly Func<TView, ValidationResult> m_validate;

        public CustomValidationRule(Func<TView, ValidationResult> validate)
        {
            this.m_validate = validate;
        }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is TView))
            {
                return new ValidationResult(false,
                    $"The value '{value}' of type '{value?.GetType()}' is not the expected type '{typeof(TView)}'.");
            }

            return this.m_validate((TView)value);
        }
    }
}