using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    /// <summary>
    /// A converter base class for values for which static mappings exist.
    /// </summary>
    /// <typeparam name="TModel">The type of the view-model side value.</typeparam>
    /// <typeparam name="TView">The type of the view side value.</typeparam>
    [New]
    public abstract class StaticValueConverter<TModel, TView> : GenericValueConverter<TModel, TView>
    {
        private (TModel ModelValue, TView ViewValue)[] Mapping => this.Get(() => this.GetMapping().ToArray());

        /// <summary>
        /// Gets the mapping between the view-model and view values.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<(TModel ModelValue, TView ViewValue)> GetMapping();

        private IEqualityComparer<TModel> ModelComparer { get; } = EqualityComparer<TModel>.Default;
        private IEqualityComparer<TView> ViewComparer { get; } = EqualityComparer<TView>.Default;

        /// <inheritdoc/>
        protected sealed override  TView Convert(TModel value, object parameter, CultureInfo culture, Type targetType)
        {
            return this.Mapping.Single(x => ModelComparer.Equals(x.ModelValue,value)).ViewValue;
        }
        /// <inheritdoc/>
        protected sealed override TModel ConvertBack(TView value, object parameter, CultureInfo culture, Type targetType)
        {
            return this.Mapping.Single(x => ViewComparer.Equals(x.ViewValue, value)).ModelValue;
        }
    }
}