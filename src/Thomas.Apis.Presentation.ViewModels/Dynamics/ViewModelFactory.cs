using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Thomas.Apis.Presentation.ViewModels.Command;

namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{
    public class ViewModelFactory
    {
        public ViewModelFactory(IViewModel viewModel, string propertyName)
        {
            ViewModel = viewModel;
            PropertyName = propertyName;
        }

        public IViewModel ViewModel { get; }
        public string PropertyName { get; }

        /// <summary>
        /// Creates or gets a threadsafe bindable collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements.</typeparam>
        /// <returns></returns>
        public ObservableCollection<T> Collection<T>(IEnumerable<T> initialItems = null)
        {

            var collection = new ObservableCollection<T>(initialItems.EmptyIfNull());
            //viewFacade.EnableCollectionSynchronization(collection);
            return collection;

        }
        public IAsyncCommand Command(Action syncAction, CommandAppearance? appearance = null, bool canExecuteWhenExecuting = false)
        {
            return new AsyncCommand<object>(async _ =>
                {
                    syncAction();
                    await Task.CompletedTask;
                }, this.ViewModel.OnError, appearance ?? this.PropertyName,
                p => Task.FromResult(canExecuteWhenExecuting ? true : !p.Command.IsExecuting));
        }


        public IAsyncCommand Command(Func<Task> asyncAction, CommandAppearance? appearance = null, bool canExecuteWhenExecuting = false)
        {
            return new AsyncCommand<object>(async _=>await asyncAction(), this.ViewModel.OnError, appearance ?? this.PropertyName,
                p => Task.FromResult(canExecuteWhenExecuting ? true : !p.Command.IsExecuting));
        }

        public IAsyncCommand<T> Command<T>(Func<T, Task> asyncAction, CommandAppearance? appearance = null, bool canExecuteWhenExecuting = false)
        {
            return new AsyncCommand<T>(asyncAction, this.ViewModel.OnError, appearance ?? this.PropertyName,
                p => Task.FromResult(canExecuteWhenExecuting ? true : !p.Command.IsExecuting));
        }
    }
}
