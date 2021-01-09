using System.IO;
using Thomas.Apis.Presentation.ViewModels.Command;
using Thomas.Apis.Presentation.ViewModels.Views;

namespace Thomas.Apis.Presentation.ViewModels.FileSystemSelection
{

    [Dynamics.View.Layout.Grid(HorizontalAutoFill =true, ColumnSizes ="*;Auto")]
    public class FileBrowserViewModel: ViewModel, IFileSelectionParameters
    {
        private IViewFacade ViewFacade { get; }

        public FileBrowserViewModel(IViewFacade viewFacade, FileSelectionMode mode, FileInfo? preselectedFile = null)
        {
            ViewFacade = viewFacade;
            this.Mode = mode;
            this.PreSelectedFile = preselectedFile;
        }
        public FileInfo? SelectedFile
        {
            get => this.Get(()=> this.PreSelectedFile);
            set => this.Set(value,_=> this.OnPropertyChanged(nameof(this.FilePath)));
        }
        public FileInfo? PreSelectedFile
        {
            get => this.Get<FileInfo?>();
            set => this.Set(value);
        }

        public FileSelectionMode Mode
        {
            get => this.Get(()=> FileSelectionMode.Open);
            set => this.Set(value);
        }

        [Dynamics.View]
        public string? FilePath
        {
            get => this.SelectedFile?.FullName;
            set => this.SelectedFile = value?.ToFileInfo();
        }

        [Dynamics.View]
        public IAsyncCommand Select =>
            this.Get(f => f.Command(() => this.SelectedFile = this.ViewFacade.SelectFile(this),"..."));

 

    }
}