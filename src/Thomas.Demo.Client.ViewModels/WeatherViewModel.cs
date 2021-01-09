using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Thomas.Apis.Presentation.ViewModels;
using Thomas.Apis.Presentation.ViewModels.Command;
using Thomas.Apis.Presentation.ViewModels.Dynamics;
using Thomas.Apis.Presentation.ViewModels.FileSystemSelection;
using Thomas.Apis.Presentation.ViewModels.Layouts;
using Thomas.Apis.Presentation.ViewModels.Views;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.ViewModels
{
    [View.Layout.Grid(ColumnSizes = "*;Auto", DefaultSizeIsAuto = true)]
    public class ErrorViewModel: ViewModel
    {
        [View.Layout.Field]
        public Icon NotificationIcon { get; } = (PackIconKind.Error, nameof(Color.Red));

        [View.Layout.Text(IsReadOnly = true)]
        public string Notification { get;  }

        public ErrorViewModel(string notification)
        {
            Notification = notification;
        }
    }
    public class WeatherViewModel : ViewModel
    {
        private WeatherQueryResponse WeatherData { get; }
        private IViewFacade ViewFacade { get; }
        public WeatherCodeService WeatherCodeService { get; }
        public WeatherRecommendationService RecommendationService { get; }

        public WeatherViewModel(WeatherQueryResponse weatherData, IViewFacade viewFacade, WeatherCodeService weatherCodeService, WeatherRecommendationService evaluationService)
        {
            WeatherData = weatherData;
            ViewFacade = viewFacade;
            WeatherCodeService = weatherCodeService;
            RecommendationService = evaluationService;
        }

        [View.Layout.Field("Recommondations")]
        public ListViewModel<WeatherRecommendationViewModel> Recommendations => this.Get(
            f => f.Collection(this.RecommendationService.Get(this.WeatherData).Select(x => new WeatherRecommendationViewModel(x.Question, x.Answer))));

        [View.Layout.Field]
        public IAsyncCommand Save => this.Get(f => f.Command(this.SaveAsync, "Save Weather Data"));

        private async Task SaveAsync()
        {
            var fileSelection = new FileBrowserViewModel(this.ViewFacade, FileSelectionMode.Save,
                Environment.SpecialFolder.Desktop.Directory().CombineFile("WeatherData.json"));
            
            await fileSelection.Select.ExecuteAsync();
            fileSelection.SelectedFile?.WriteContent(JsonConvert.SerializeObject(this.WeatherData));
        }
    }

}
