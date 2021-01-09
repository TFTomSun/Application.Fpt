using System.Collections.Generic;

namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    public interface IMultiSelectionViewModel:IViewModel
    {
        bool IsSelectionVisible { get; set; }
        IEnumerable<ISelectableItemViewModel> SelectionModels { get; }

        ISelectableItemViewModel FocusedModel { get; }
    }
}