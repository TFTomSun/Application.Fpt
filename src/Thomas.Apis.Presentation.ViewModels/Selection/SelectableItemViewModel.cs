namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    public class SelectableItemViewModel<TItem>: ViewModel<SelectableItemViewModel<TItem>>, ISelectableItemViewModel
    {
        public IViewModel Parent { get; }

        public SelectableItemViewModel(IViewModel parent)
        {
            Parent = parent;
        }
        public bool? IsSelected
        {
            get => this.Get<bool?>(()=> false);
            set => this.Set(value);
        }

        public TItem Value
        {
            get => this.Get<TItem>();
            set => this.Set(value);
        }

        public override string DisplayText => this.Value.ToString();
        object ISelectableItemViewModel.Value {get => this.Value; }
    }
}