using CrazySharp.Std;
using Newtonsoft.Json;

namespace khwkit_tools.Beans
{
    public class UpdateSsmRemoteCtlInfoReq
    {
        [JsonProperty("remoteType")]
        public string RemoteType { get; set; }
        [JsonProperty("detail")]
        public JsonObject Detail { get; set; }
    }
}
