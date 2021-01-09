using System.Collections.Generic;
using System.Windows;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    public class BoolNegationConverter : StaticValueConverter<bool?, bool?>
    {
        protected override IEnumerable<(bool? ModelValue, bool? ViewValue)> GetMapping()
        {
            yield return (true, false);
            yield return (false, true);
            yield return (null, null);
        }
    }
    /// <summary>
    /// A value converter from
    /// </summary>
    [New]
    public class BoolToTextWrappingConverter : StaticValueConverter<bool?,TextWrapping>
    {
        /// <inheritdoc/>
        protected override IEnumerable<(bool? ModelValue, TextWrapping ViewValue)> GetMapping()
        {
            yield return (true,TextWrapping.Wrap );
            yield return (false,TextWrapping.NoWrap );
            yield return (null,TextWrapping.WrapWithOverflow);
        }
    }
}