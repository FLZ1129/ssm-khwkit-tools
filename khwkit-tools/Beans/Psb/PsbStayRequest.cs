using Newtonsoft.Json;

namespace khwkit.Beans.Psb
{
    public class PsbStayRequest
    {
        /// <summary>
        /// 身份号
        /// </summary>
        [JsonProperty("number")]
        public string Number { get; set; }

        /// <summary>
        /// 在住房间号
        /// </summary>
        [JsonProperty("roomNo")]
        public string RoomNo { get; set; }

        /// <summary>
        /// 离店时间
        /// </summary>
        [JsonProperty("outTime")]
        public long OutTime { get; set; }
    }

}
