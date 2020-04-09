using Newtonsoft.Json;

namespace khwkit.Beans.IdCard
{
    /// <summary>
    /// 用户身份证信息
    /// </summary>
    public class IdCardInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别，当指定为护照识别时此项为空
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 地址，在识别护照时导出的是国籍简码
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 签发机关
        /// </summary>
        public string IssuingAuthority { get; set; }
        /// <summary>
        /// 有效日期开始
        /// </summary>
        public string ValidBegin { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        public string ValidEnd { get; set; }
        /// <summary>
        /// 安全模块号码
        /// </summary>
        [JsonIgnore]
        public string SamId { get; set; }
        /// <summary>
        /// 身份证物理卡号
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 照片存储位置
        /// </summary>
        [JsonIgnore] 
        public string IdPicFile { get; set; }
        /// <summary>
        /// 照片Base64编码
        /// </summary>
        public string ImageBase64 { get; set; }

        public override string ToString()
        {
            return $"{Name} | {Number} | {Sex} | {Nation} | {Address} | {ValidBegin}-{ValidEnd}";
        }
    }
}
