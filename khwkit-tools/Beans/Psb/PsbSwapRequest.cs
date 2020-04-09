using Newtonsoft.Json;

namespace khwkit.Beans.Psb
{
    public class PsbSwapRequest
    {
        /// <summary>
        /// 身份号
        /// </summary>
        [JsonProperty("number")]
        public string Number { get; set; }

        /// <summary>
        /// 在住房间号
        /// </summary>
        [JsonProperty("oldRoomNo")]
        public string OldRoomNo { get; set; }

        /// <summary>
        /// 目标房间号
        /// </summary>
        [JsonProperty("newRoomNo")]
        public string NewRoomNo { get; set; }
    }

}
