using System.Collections.Generic;
using System.Windows;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
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