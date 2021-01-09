using System.ComponentModel;

namespace Thomas.Apis.Presentation.ViewModels
{
    public interface INotifyObject: INotifyPropertyChanged
    {
        void RaisePropertyChanged(string propertyName, bool resetMember = false);
    }
}