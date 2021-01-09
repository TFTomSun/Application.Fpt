using System.Collections.Generic;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    [New]
    public interface IItemsViewModel
    {
        IEnumerable<object> Items { get; }
    }
}
