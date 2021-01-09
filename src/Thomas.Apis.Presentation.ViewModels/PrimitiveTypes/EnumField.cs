using System;
using System.Linq;
using Thomas.Apis.Presentation.ViewModels.Dynamics;
using Thomas.Apis.Presentation.ViewModels.Selection;

namespace Thomas.Apis.Presentation.ViewModels.PrimitiveTypes
{
    [View.Layout.Grid]
    public class EnumField : MetaViewModel<Enum>
    {
        [View]
        public SelectionViewModel<Enum?> SelectionModel => this.Get(() => new SelectionViewModel<Enum?>(
            Enum.GetValues(this.ContextPropertyType).OfType<Enum?>(),
            e => e.ToString(), s => (Enum?) Enum.Parse(this.ContextPropertyType, s),
            (Enum?) this.ContextPropertyType.GetDefaultValue(),
            (a, b) => object.Equals(a, b)));

        public override bool Supports(MetaViewModelContext context)
        {
            return context.PropertyType.IsEnum || context.PropertyType.IsNullableEnum();
        }
    }
}