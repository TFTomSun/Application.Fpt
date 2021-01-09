using System.Windows.Controls;
using System.Windows.Data;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    /// <summary>
    /// A typesafe generic WPF value converter.
    /// </summary>
    /// <typeparam name="TModel">The type of the view-model side value.</typeparam>
    /// <typeparam name="TView">The type of the view side value.</typeparam>
    [New("Toggled generic arguments")]
    public abstract class GenericValueConverter<TModel,TView> : CommonValueConverter<TModel, TView>,
        IValueConverter, IValidationRuleProvider
    {

        public ValidationRule Validation
            => this.Get(() => new CustomValidationRule<TView>(this.Validate));

        protected virtual ValidationResult Validate(TView value) => new ValidationResult(true, null);

    }
}