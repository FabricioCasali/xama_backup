using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigRetention
    {

        [JsonProperty("fullCopiesToKeep")]
        public int NumberOfFullCopies { get; set; }

        [JsonProperty("makeFullCopieEvery")]
        public int FullCopieEvery { get; set; }
    }

}
