using System.IO;
using Thomas.Apis.Presentation.ViewModels.FileSystemSelection;

namespace Thomas.Apis.Presentation.ViewModels.Views
{
    public interface IViewFacade
    {
        FileInfo? SelectFile(IFileSelectionParameters viewModel);
        bool? ShowMessageBox(string title, string text);
    }
}