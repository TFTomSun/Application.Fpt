using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thomas.Apis.Core;
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
    public class WeatherViewModel : ViewModel
    {
        public WeatherViewModel(WeatherQueryResponse weatherData, IViewFacade viewFacade, WeatherCodeService weatherCodeService, WeatherRecommendationService evaluationService)
        {
            WeatherData = weatherData;
            ViewFacade = viewFacade;
            WeatherCodeService = weatherCodeService;
            EvaluationService = evaluationService;
        }

        [View.Layout.Field("Recommondations")]
        public ListViewModel<WeatherRecommendationViewModel> Recommondations => this.Get(
            f => f.Collection(this.EvaluationService.Get(this.WeatherData).Select(x => new WeatherRecommendationViewModel(x.Question, x.Answer))));

        [View.Layout.Field]
        public IAsyncCommand Save => this.Get(f => f.Command(this.SaveAsync, "Save Weather Data"));


        public WeatherQueryResponse WeatherData { get; }
        public IViewFacade ViewFacade { get; }
        public WeatherCodeService WeatherCodeService { get; }
        public WeatherRecommendationService EvaluationService { get; }


        public async Task SaveAsync()
        {
            var fileSelection = new FileBrowserViewModel(this.ViewFacade, FileSelectionMode.Save,
                Environment.SpecialFolder.Desktop.Directory().CombineFile("WeatherData.json"));
            
            await fileSelection.Select.ExecuteAsync();
            fileSelection.SelectedFile?.WriteContent(JsonConvert.SerializeObject(this.WeatherData));
        }

    }

}
