using System;

namespace khwkit.Beans.KitSystem
{
    public enum KitPower
    {
        /// <summary>
        /// 断电
        /// </summary>
        OFF = 0x01,
        /// <summary>
        /// 上电
        /// </summary>
        ON = 0x02,
        /// <summary>
        /// 重启
        /// </summary>
        REBOOT = 0x03
    }
    [Flags]
    public enum KitDevices
    {
        /// <summary>
        /// win主板
        /// </summary>
        Windows = 0x01,
        /// <summary>
        /// Android主板
        /// </summary>
        Android = 0x02,
        /// <summary>
        /// USB电源(摄像头等)
        /// </summary>
        USBPower = 0x04,
        /// <summary>
        /// 12v电源(风扇)
        /// </summary>
        Power12V = 0x08,
        /// <summary>
        ///  24v电源(无设备)
        /// </summary>
        Power24V = 0x10,
        /// <summary>
        /// 干接点1(小票打印机)
        /// </summary>
        POWER_DRY_1 = 0x20,
        /// <summary>
        /// 干接点2(收发卡机)
        /// </summary>
        POWER_DRY_2 = 0x40,
        /// <summary>
        /// 所有电源
        /// </summary>
        All = 0x7f
    }
    public class KitPowerCtrlRequest
    {
        /// <summary>
        ///  1 -> 断电 ; 2 - >上电 ; 3 ->  重启
        /// </summary>
        public KitPower Action { get; set; }
        /// <summary>
        ///  0x1 -> win主板  ; 0x2  -> Android主板 ; 0x4 -> USB电源 ; 0x8 -> 12v电源  ; 0x10 -> 24v电源 ; 0x7F  -> 所有电源
        /// </summary>
        public KitDevices Devices { get; set; }
    }
}
