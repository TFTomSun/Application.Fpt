using System;

namespace Thomas.Apis.Presentation.ViewModels
{
    public interface IViewModel: INotifyObject
    {
        void OnError(Exception ex);
    }
}
