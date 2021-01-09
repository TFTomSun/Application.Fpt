using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Thomas.Apis.Presentation.ViewModels.FileSystemSelection;
using Thomas.Apis.Presentation.ViewModels.Views;

namespace Thomas.Apis.Presentation.Wpf.Views
{
    public class DefaultViewFacade: IViewFacade
    {
        private UIElement Host { get; }

        public DefaultViewFacade(UIElement host)
        {
            Host = host;
        }

        public FileInfo? SelectFile(IFileSelectionParameters viewModel)
        {
            var dialog = viewModel.Mode == FileSelectionMode.Open ? new OpenFileDialog().As<FileDialog>() : new SaveFileDialog();
            dialog.InitialDirectory = viewModel.PreSelectedFile?.Directory.FullName;
            dialog.FileName = viewModel.PreSelectedFile?.Name;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName.ToFileInfo();
            }
            return null;
        }

        public bool? ShowMessageBox(string title, string text)
        {
            var result = System.Windows.MessageBox.Show(text, title, MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }
    }

 
}
