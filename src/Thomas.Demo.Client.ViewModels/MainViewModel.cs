using System;
using Thomas.Apis.Presentation.ViewModels;
using Thomas.Apis.Presentation.ViewModels.Command;
using Thomas.Apis.Presentation.ViewModels.Dynamics;
using Thomas.Apis.Presentation.ViewModels.Views;
using Thomas.Demo.Client.Services.WeatherStack;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.ViewModels
{
    /// <summary>
    /// The main view model of the client.
    /// </summary>
    [View.Layout.Grid.Cell(Margin = "5", InheritToChildren = true)]
    [View.Layout.Grid(ColumnSizes = "*;Auto",  DefaultSizeIsAuto=true)]
    public class MainViewModel : ViewModel
    {
        private IViewFacade ViewFacade { get; }
        public WeatherStackService WeatherService { get; }
        public Func<ServiceResponse, WeatherResponsViewModel> WeatherResponseViewModelFactory { get; }

        /// <summary>
        /// Creates an instance of the <see cref="MainViewModel"/>
        /// </summary>
        /// <param name="viewFacade">The dependency inversion interface for the view.</param>
        public MainViewModel(IViewFacade viewFacade, WeatherStackService weatherService, Func<ServiceResponse,WeatherResponsViewModel> weatherResponseViewModelFactory)
        {
            ViewFacade = viewFacade;
            this.WeatherService = weatherService;
            WeatherResponseViewModelFactory = weatherResponseViewModelFactory;
        }

        /// <summary>
        /// Gets or sets Main Window title.
        /// </summary>
        public string? Title
        {
            get => this.Get(()=> "Weather Demo Client");
            set => this.Set(value);
        }


        /// <summary>
        /// Gets or sets the Server Address
        /// </summary>
        //[View.Layout.Frame(Margin = Margin)]
        [View.Layout.Field("City Name or ZipCode")]
        public string? CityNameOrZipCode
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        [View.Layout.Field]
        public IAsyncCommand Query => this.Get(f => f.Command(
            async ()=>this.Result =this.WeatherResponseViewModelFactory(
                await this.WeatherService.QueryCurrentAsync(this.CityNameOrZipCode)), "Show Weather"));


        [View.Layout.Grid.Cell(ColumnSpan =2)]
        public WeatherResponsViewModel Result
        {
            get => this.Get<WeatherResponsViewModel>();
            set => this.Set(value);

        }
        
    }

}
