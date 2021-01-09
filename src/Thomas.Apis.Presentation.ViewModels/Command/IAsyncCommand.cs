using System.Threading.Tasks;
using System.Windows.Input;

namespace Thomas.Apis.Presentation.ViewModels.Command
{
    public interface IAsyncCommand : IViewModel
    {
        bool CanExecute { get; set; }
        CommandAppearance Appearance { get; set; }
        ICommand CoreCommand { get; }

        bool IsExecuting { get; internal set; }

        Task ExecuteAsync();
    }
    public interface IAsyncCommand<TParameter> : IAsyncCommand
    {
        Task ExecuteAsync(TParameter parameter);
    }
}
