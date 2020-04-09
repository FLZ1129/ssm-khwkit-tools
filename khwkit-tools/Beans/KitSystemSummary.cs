using Newtonsoft.Json;

namespace khwkit.Beans
{
    public class KitSystemSummary
    {
        [JsonProperty("SN")]
        public string SN { get; set; }
        public string SwVersion { get; set; }
        public string HwVersion { get; set; }
    }
}
