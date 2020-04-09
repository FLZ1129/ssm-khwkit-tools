using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace khwkit.Beans.RoomCard
{
    public class LockConfigRequest
    {
        public string RoomNo { get; set; }
        /// <summary>
        /// 门锁锁号地址
        /// </summary>
        public string LockAddress { get; set; }
        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// 楼栋（区域）
        /// </summary>
        public string Building { get; set; }
        /// <summary>
        /// 楼层
        /// </summary>
        public string Floor { get; set; }
    }
}
