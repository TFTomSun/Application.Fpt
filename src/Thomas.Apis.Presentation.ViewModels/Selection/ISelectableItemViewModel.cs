namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    public interface ISelectableItemViewModel : IViewModel
    {
        IViewModel Parent { get; }
        bool? IsSelected { get; set; }

        object Value { get;  }

    }
}