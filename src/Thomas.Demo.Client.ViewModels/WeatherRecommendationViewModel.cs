using System.Drawing;
using Thomas.Apis.Presentation.ViewModels;
using Thomas.Apis.Presentation.ViewModels.Command;
using Thomas.Apis.Presentation.ViewModels.Dynamics;
using Thomas.Apis.Presentation.ViewModels.Views;

namespace Thomas.Demo.Client.ViewModels
{
    [View.Layout.Grid(ColumnSizes = "*;Auto", DefaultSizeIsAuto = true)]
    public class WeatherRecommendationViewModel :ViewModel
    {
        public WeatherRecommendationViewModel(string question, bool answer)
        {
            Question = question;
            this.Answer = answer ? (PackIconKind.Check, nameof(Color.Green)) :(PackIconKind.Denied, nameof(Color.Red));
        }

        [View.Layout.Text(IsReadOnly =true)]
        public string Question { get; }

        [View.Layout.Field]
        public Icon Answer { get; }
    }

}
