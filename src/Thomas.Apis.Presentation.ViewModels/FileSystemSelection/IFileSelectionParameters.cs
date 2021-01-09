using System.IO;

namespace Thomas.Apis.Presentation.ViewModels.FileSystemSelection
{
    public interface IFileSelectionParameters
    {
        FileInfo? PreSelectedFile { get; }
        FileSelectionMode Mode { get; }
    }
}