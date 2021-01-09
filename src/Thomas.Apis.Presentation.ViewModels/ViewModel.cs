using System;
using System.Runtime.CompilerServices;
using Thomas.Apis.Core.DotNet;
using Thomas.Apis.Presentation.ViewModels.Dynamics;

namespace Thomas.Apis.Presentation.ViewModels
{
    public abstract class ViewModel<TSelf>:ViewModel
    {

    }
    /// <summary>
    /// A base class for view models.
    /// </summary>
    public abstract class ViewModel : NotifyObject, IViewModel
    {
        public virtual string DisplayText { get; }

        protected DisposableContainer Disposables { get; set; } = new DisposableContainer();

        public static Action<Exception> ExceptionThrown;

        protected T Get<T>(Func<ViewModelFactory,T> getDefault, [CallerMemberName] string? propertyName = null)
        {
            return  this.Get(() => getDefault(new ViewModelFactory(this, propertyName.NullCheck())), propertyName) ?? default!;
        }


        public virtual void OnError(Exception ex)
        {
            ExceptionThrown?.Invoke(ex);
        }

    }
}
