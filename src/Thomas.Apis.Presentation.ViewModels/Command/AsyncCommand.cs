using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Thomas.Apis.Presentation.ViewModels.Command
{
    public class AsyncCommand<TParameter> : ViewModel, ICommand, IAsyncCommand<TParameter>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="asyncAction">The async action that is wrapped by this command</param>
        /// <param name="onError">Defines the error handling.</param>
        /// <param name="appearance">Some addition information for the command appearance.</param>
        /// <param name="canExecuteAsync">A function to check whether the command can be executed. If not specified, the command will always be executable.</param>
        public AsyncCommand(
            Func<TParameter, Task> asyncAction, Action<Exception> onError, CommandAppearance appearance,
            Func<(TParameter Parameter, IAsyncCommand<TParameter> Command), Task<bool>>? canExecuteAsync = null)
        {
            AsyncAction = asyncAction;
            OnError = onError;
            Appearance = appearance;
            CanExecuteAsync = canExecuteAsync ?? (_ => Task.FromResult(true));
        }

        private Func<TParameter, Task> AsyncAction { get; }
        private new Action<Exception> OnError { get; }

        /// <summary>
        /// Gets the bindable appearance.
        /// </summary>
        public CommandAppearance? Appearance
        {
            get => this.Get(() => new CommandAppearance());
            set => this.Set(value);
        }
        private Func<(TParameter Parameter, IAsyncCommand<TParameter> Command), Task<bool>> CanExecuteAsync { get; }

        /// <summary>
        /// Will be invoked, whenever the can execute check changed.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Gets the current state of the can execute check.
        /// </summary>
        public bool CanExecute
        {
            get => this.Get(()=> true);
            set => this.Set(value, _ => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public ICommand CoreCommand => this;


        /// <summary>
        /// Gets or sets a value that indicates whether the command is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get => this.Get(() => false);
            private set => this.Set(value);
        }

        public async Task ExecuteAsync()
        {
            await this.ExecuteAsync(default!);
        }


        bool IAsyncCommand.IsExecuting
        {
            get => this.IsExecuting;
            set => this.IsExecuting = value;
        }

        private async void UpdateCanExecuteAsync(TParameter parameter)
        {
            this.CanExecute = await this.CanExecuteAsync((parameter,this));
        }

        /// <summary>
        /// Allows to execute the command programmatically.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns></returns>
        public async Task ExecuteAsync(TParameter parameter)
        {
            this.IsExecuting = true;
            try
            {
                this.UpdateCanExecuteAsync(parameter);
                await this.AsyncAction(parameter);
            }
            finally
            {
                this.IsExecuting = false;
                this.UpdateCanExecuteAsync(parameter);
            }
        }

       


        bool ICommand.CanExecute(object parameter)
        {
            try
            {
                this.UpdateCanExecuteAsync((TParameter)parameter);
                return this.CanExecute;
            }
            catch (Exception ex)
            {
                this.OnError(ex);
                return this.CanExecute;
            }
        }

        async void ICommand.Execute(object parameter)
        {
            try
            {
                await this.ExecuteAsync((TParameter)parameter);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }
    }
}
