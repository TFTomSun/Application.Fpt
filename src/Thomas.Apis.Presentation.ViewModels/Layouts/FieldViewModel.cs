using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.ViewModels.Dynamics;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    [New]
    public class FieldViewModel : ContentMetaViewModel< View.Layout.Field>
    {
        public string? Label
        {
            get => this.Get(()=>this.Settings?.Label);
            set => this.Set(value);
        }

        public override int Rank => 10;
     
    }
}