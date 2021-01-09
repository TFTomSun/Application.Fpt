using System.Collections.Generic;
using System.Collections.ObjectModel;
using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    [New]
    public class ListViewModel<T>: ViewModel, IItemsViewModel
    {

        public ICollection<T> Items
        {
            get => this.Get(f => f.Collection<T>());
            private set => this.Set(value);
        }

        IEnumerable<object> IItemsViewModel.Items => (this.Items?.AsObjectEnumerable()).EmptyIfNull();

        public static implicit operator ListViewModel<T>(ObservableCollection<T> collection)
        {

            return new ListViewModel<T> { Items = collection };
        }
    }
}
