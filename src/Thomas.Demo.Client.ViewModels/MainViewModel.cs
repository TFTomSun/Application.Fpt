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
        private WeatherStackService WeatherService { get; }
        private Func<WeatherQueryResponse, WeatherViewModel> WeatherResponseViewModelFactory { get; }

        /// <summary>
        /// Creates an instance of the <see cref="MainViewModel"/>
        /// </summary>
        public MainViewModel(WeatherStackService weatherService, Func<WeatherQueryResponse,WeatherViewModel> weatherResponseViewModelFactory)
        {
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
        /// Gets or sets the city or zip code for which the weather data shall be retrieved.
        /// </summary>
        [View.Layout.Field("City Name or ZipCode")]
        public string? CityNameOrZipCode
        {
            get => this.Get<string>(()=>"New York");
            set => this.Set(value);
        }

        /// <summary>
        /// Triggers the query for the weather data.
        /// </summary>
        [View.Layout.Field]
        public IAsyncCommand Query => this.Get(f => f.Command(
            async ()=>this.Result =this.WeatherResponseViewModelFactory(
                await this.WeatherService.QueryCurrentAsync(this.CityNameOrZipCode)), "Show Weather"));


        /// <summary>
        /// Will be set when the weather data has been queried.
        /// </summary>
        [View.Layout.Grid.Cell(ColumnSpan =2)]
        public WeatherViewModel Result
        {
            get => this.Get<WeatherViewModel>();
            private set => this.Set(value);
        }
        
    }

}
