using CrazySharp.Std;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace khwkit.Core
{
    public class KitServiceConfigReq
    {
        [JsonProperty("currentProviderId")]
        public string CurrentProviderId { get; set; }
        [JsonProperty("props")]
        public JsonObject Props {get;set;}
    }
    
    public class KitServiceItem
    {
        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }
        [JsonProperty("serviceName")]
        public string ServiceName { get; set; }
        [JsonProperty("serviceDesc")]
        public string ServiceDesc { get; set; }
        [JsonProperty("currentProvider")]
        public KitServiceProviderItem CurrentProvider { get;  set; }
        [JsonProperty("serviceProviders")]
        public List<KitServiceProviderItem> ServiceProviders { get; set; }
    }
}
