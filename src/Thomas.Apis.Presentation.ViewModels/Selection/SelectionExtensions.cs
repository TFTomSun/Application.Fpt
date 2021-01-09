using System;
using System.Linq;

namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    public static class SelectionExtensions
    {
        public static SelectionViewModel<TEnum?> ToSelectionViewModel<TEnum>(this TEnum value)
            where TEnum : struct,Enum
        {
            return new SelectionViewModel<TEnum?>(
                default(TEnum).GetValues().Cast<TEnum?>(),
                e => e.ToString(), s =>  s.TryToEnum<TEnum>(),
                value,
                (a, b) => object.Equals(a, b));
        }

  
    }
}