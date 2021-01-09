using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.ViewModels.Dynamics;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    [New]
    public class GridCellViewModel : ContentMetaViewModel<View.Layout.Grid.Cell>
    {
        public override int Rank => 12;

        public override View.Layout.Grid.Cell? Settings
        {
            get
            {
                var settings = base.Settings;
                return settings;
            }
        }
    }
    [New]
    public class FrameViewModel : ContentMetaViewModel<View.Layout.Frame>
    {
        public override int Rank => 11;
    }
}