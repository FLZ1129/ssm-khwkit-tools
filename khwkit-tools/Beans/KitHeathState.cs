using Newtonsoft.Json;

namespace khwkit.Beans
{
    public class KitHeathState
    {
        [JsonProperty("state")]
        public string State { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [JsonProperty("stateDesc")]
        public string StateDesc { get; set; }
    }
}
