using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.ViewModels.Command
{
    [New]
    public class Progress : ViewModel
    {
        public bool IsIndeterminate
        {
            get => this.Get<bool>(() => true);
            set => this.Set(value);
        }

        public double Value
        {
            get => this.Get<double>();
            set => this.Set(value, x => this.IsIndeterminate = false);
        }
    }
}