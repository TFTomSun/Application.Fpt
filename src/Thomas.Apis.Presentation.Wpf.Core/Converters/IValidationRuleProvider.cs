using System.Windows.Controls;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    public interface IValidationRuleProvider
    {
        ValidationRule Validation { get; }
    }
}