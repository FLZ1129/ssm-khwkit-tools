using Newtonsoft.Json;

namespace khwkit.Beans
{
    public class KitSystemSummary
    {
        /// <summary>
        /// 序列号
        /// </summary>
        [JsonProperty("SN")]
        public string SN { get; set; }
        /// <summary>
        /// 软件版本
        /// </summary>
        [JsonProperty("swVersion")]
        public string SwVersion { get; set; }
        /// <summary>
        /// 扩展板硬件版本
        /// </summary>
        [JsonProperty("hwVersion")]
        public string HwVersion { get; set; }
        /// <summary>
        /// 扩展板硬件版本说明
        /// </summary>
        [JsonProperty("hwVersionDesc")]
        public string HwVersionDesc { get; set; }
    }
}
