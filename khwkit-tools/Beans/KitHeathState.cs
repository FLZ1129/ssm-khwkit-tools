using Newtonsoft.Json;

namespace khwkit.Beans
{
    public enum HeathState
    {
        NORMAL,
        WARN,
        ERROR
    }
    public class KitHeathState
    {
        [JsonProperty("state")]
        public string StateStr => State.ToString();
        /// <summary>
        /// 状态 Normal Warn Error
        /// </summary>
        [JsonIgnore]
        public HeathState State { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [JsonProperty("stateDesc")]
        public string StateDesc { get; set; }
    }
}
