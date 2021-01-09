using System.Collections.Generic;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.Services.WeatherStack
{
    public class WeatherRecommendationService
    {
        public IEnumerable<(string Question, bool Answer)> Get(WeatherQueryResponse weatherQueryResponse)
        {
            var current = weatherQueryResponse.current;
            yield return ("Should I go out?", !current.IsRaining());
            yield return ("Should I wear sunscreen?", current.uv_index > 3);
            yield return ("Can I fly my kite?", !current.IsRaining() && current.wind_speed > 15);
        }


    }
    

   
}
