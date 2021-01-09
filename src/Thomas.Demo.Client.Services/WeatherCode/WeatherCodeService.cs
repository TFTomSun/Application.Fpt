using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Thomas.Apis.Core.Extendable;
using Thomas.Demo.Client.Services.WeatherStack.Model;

namespace Thomas.Demo.Client.Services.WeatherStack
{
    public class WeatherCodeService
    {
        private static IExtendableObject StaticCache { get; } = new ExtendableObject();

        private codes WeatherCodes => StaticCache.Get(() => (codes)new XmlSerializer(typeof(codes)).Deserialize(
            new StringReader(typeof(WeatherCodeService).Assembly.GetResourceFileContent("wwoConditionCodes.xml"))));

        public Task<codesCondition> ResolveCodeAsync(int weatherCode) => Task.FromResult(this.WeatherCodes.condition.Single(
            c => c.code == weatherCode, $"the weather code '{weatherCode}' couldn't be resolved"));
    }
    

   
}
