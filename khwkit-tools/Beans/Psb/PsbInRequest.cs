using Newtonsoft.Json;

namespace khwkit.Beans.Psb
{
    public class PsbInRequest
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [JsonProperty("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// 名族
        /// </summary>
        [JsonProperty("nation")]
        public string Nation { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// 身份号
        /// </summary>
        [JsonProperty("number")]
        public string Number { get; set; }

        /// <summary>
        /// 身份证住址
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 签发机关
        /// </summary>
        [JsonProperty("grantDept")]
        public string GrantDept { get; set; }

        /// <summary>
        /// 身份证有效起始日期
        /// </summary>
        [JsonProperty("validBegin")]
        public string ValidBegin { get; set; }

        /// <summary>
        /// 身份证有效截止日期
        /// </summary>
        [JsonProperty("validEnd")]
        public string ValidEnd { get; set; }

        /// <summary>
        /// 身份证头像
        /// </summary>
        [JsonProperty("photo")]
        public string Photo { get; set; }

        /// <summary>
        /// 现场图片
        /// </summary>
        [JsonProperty("curPhoto")]
        public string CurPhoto { get; set; }

        /// <summary>
        /// 入住时间
        /// </summary>
        [JsonProperty("inTime")]
        public long InTime { get; set; }

        /// <summary>
        /// 离店时间
        /// </summary>
        [JsonProperty("outTime")]
        public long OutTime { get; set; }

        /// <summary>
        /// 入住房间号
        /// </summary>
        [JsonProperty("roomNo")]
        public string RoomNo { get; set; }

    }
}
