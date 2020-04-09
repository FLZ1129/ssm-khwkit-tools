using System.Collections.Generic;

namespace khwkit.Errors
{
    public enum KitErrCode
    {
        Success = 0,
        ErrBadRequest = 1400,
        ErrNotFound = 1404,
        ErrInterServcerErr = 1500,
        /// <summary>
        /// 请求的硬件繁忙
        /// </summary>
        ErrServiceBusy = 1503,
        /// <summary>
        /// 命令执行失败
        /// </summary>
        ErrDeviceExeFailed = 2000,
        /// <summary>
        /// 设备连接失败
        /// </summary>
        ErrKitConnectFailed = 2500,
        /// <summary>
        /// 设备状态异常
        /// </summary>
        ErrKitDevStateErr = 2501,
        /// <summary>
        /// 命令执行超时
        /// </summary>
        ErrKitDevExeTimeoutErr = 2503,

        #region 收发卡机
        /// <summary>
        /// 收发卡机连接失败
        /// </summary>
        ErrCardBoxConnectFailed = 5000,
        /// <summary>
        ///  卡箱卡已空
        /// </summary>
        ErrCardBoxEmpty = 5001,
        /// <summary>
        ///  卡箱卡已满
        /// </summary>
        ErrCardBoxFull = 5002,
        /// <summary>
        /// 取卡位有卡未取走
        /// </summary>
        ErrCardBoxCardNotTake = 5003,
        /// <summary>
        /// 超时未插卡
        /// </summary>
        ErrCardBoxCardNotPut = 5004,
        /// <summary>
        /// 命令执行出错
        /// </summary>
        ErrCardBoxCmdExeFailed=5005,
        /// <summary>
        /// 命令执行超时
        /// </summary>
        ErrCardBoxCmdExeTimeout = 5006,
        /// <summary>
        /// 读写卡位置有卡
        /// </summary>
        ErrCardAtReadWrite=5007,

        #endregion

        #region 身份证阅读器
        /// <summary>
        /// 身份证阅读器连接失败
        /// </summary>
        ErrIDReadConnectFailed = 5100,
        /// <summary>
        /// 身份证阅读器读取超时
        /// </summary>
        ErrIDReadTimeOut = 5101,
        /// <summary>
        /// 身份证读取失败
        /// </summary>
        ErrIDReadFailed = 5102,
        /// <summary>
        /// 身份证读取取消
        /// </summary>
        ErrIDReadCanceled = 5103,
        #endregion

        #region 旅业
        /// <summary>
        /// 旅业数据库连接失败
        /// </summary>
        ErrPsbSqlConnect = 5200,
        /// <summary>
        /// 入住上传失败
        /// </summary>
        ErrPsbCheckIn = 5201,
        /// <summary>
        /// 离店上传失败
        /// </summary>
        ErrPsbCheckOut = 5202,
        /// <summary>
        /// 换房上传失败
        /// </summary>
        ErrPsbSwap = 5203,
        /// <summary>
        /// 续住上传失败
        /// </summary>
        ErrPsbStay = 5204,
        /// <summary>
        /// 未找到入住记录
        /// </summary>
        ErrPsbGuestNotExist=5205,
        /// <summary>
        /// 调用PSB服务超时
        /// </summary>
        ErrPsbCallTimeout=5206,
        #endregion

        #region 小票打印机
        /// <summary>
        /// 小票打印机连接失败
        /// </summary>
        ErrTicketPrinterConnect = 5300,
        /// <summary>
        /// 纸尽，无纸
        /// </summary>
        ErrTicketPrinterPaperOut = 5301,
        /// <summary>
        /// 纸少
        /// </summary>
        ErrTicketPrinterPaperLess = 5302,
        #endregion

        #region 读写卡
        /// <summary>
        /// 读写卡初始化失败
        /// </summary>
        ErrRoomCardInit = 5400,
        /// <summary>
        /// 制卡失败
        /// </summary>
        ErrRoomCardMake = 5401,
        /// <summary>
        /// 复制卡失败
        /// </summary>
        ErrRoomCardCopy = 5402,
        /// <summary>
        /// 读卡失败
        /// </summary>
        ErrRoomCardRead = 5403,
        /// <summary>
        /// 消除卡片失败
        /// </summary>
        ErrRoomCardClear = 5404,
        /// <summary>
        /// 读写卡超时
        /// </summary>
        ErrRoomCardTimeOut = 5405,
        /// <summary>
        /// 制卡失败
        /// </summary>
        ErrRoomCardWrite = 5406,
        #endregion

        #region QRScanner
        /// <summary>
        /// 二维码扫描器连接失败
        /// </summary>
        ErrQRScannerConnectFailed=5500,
        /// <summary>
        ///  二维码扫描器初始化失败
        /// </summary>
        ErrQRScanInitFaild = 5501,
        /// <summary>
        /// 二维码扫描器扫描超时
        /// </summary>
        ErrQRScanTimeout = 5502,
        /// <summary>
        /// 扫描已取消
        /// </summary>
        ErrQRScanCanceled = 5503,
        /// <summary>
        /// 扫描命令执行失败
        /// </summary>
        ErrQRScanExecuteFailed = 5504,
        #endregion
    }

    public static class KitErrExtensions
    {
        private static readonly Dictionary<KitErrCode, string> _kitCodeMsgs = new Dictionary<KitErrCode, string> {
           { KitErrCode.Success, "Success" },
           { KitErrCode.ErrBadRequest, "Bad Request" },
           { KitErrCode.ErrNotFound, "Service not config yet" },
           { KitErrCode.ErrInterServcerErr, "Internal error" },
           { KitErrCode.ErrServiceBusy, "请求的硬件繁忙" },
           { KitErrCode.ErrDeviceExeFailed, "命令执行失败" },
           { KitErrCode.ErrKitConnectFailed, "设备连接失败" },
           { KitErrCode.ErrKitDevStateErr, "设备状态异常" },
           { KitErrCode.ErrKitDevExeTimeoutErr, "命令执行超时" },

           { KitErrCode.ErrCardBoxConnectFailed, "收发卡机连接失败" },
           { KitErrCode.ErrCardBoxEmpty, "卡箱卡已空" },
           { KitErrCode.ErrCardBoxCardNotTake, "取卡位有卡未取走" },
           { KitErrCode.ErrCardBoxCardNotPut, " 超时未插卡" },
           { KitErrCode.ErrCardBoxCmdExeFailed, "命令执行出错" },
           { KitErrCode.ErrCardBoxCmdExeTimeout, "命令执行超时" },
            {KitErrCode.ErrCardAtReadWrite,"读写卡位置有卡" },

           {KitErrCode.ErrIDReadConnectFailed,"身份证阅读器连接失败" },
           {KitErrCode.ErrIDReadTimeOut,"身份证信息读取超时" },
           {KitErrCode.ErrIDReadFailed,"身份证信息读取失败" },
           {KitErrCode.ErrIDReadCanceled,"身份证信息读取已被取消" },

           {KitErrCode.ErrPsbSqlConnect,"旅业数据库连接失败"},
           {KitErrCode.ErrPsbCheckIn,"旅业入住上传失败"},
           {KitErrCode.ErrPsbCheckOut,"旅业离店上传失败"},
           {KitErrCode.ErrPsbSwap,"换房上传失败"},
           {KitErrCode.ErrPsbStay,"续住上传失败"},
           {KitErrCode.ErrPsbGuestNotExist,"未找到入住记录"},
           {KitErrCode.ErrPsbCallTimeout,"调用PSB服务超时"},

           {KitErrCode.ErrTicketPrinterConnect,"小票打印机连接失败" },
           {KitErrCode.ErrTicketPrinterPaperLess,"小票打印机少纸" },
           {KitErrCode.ErrTicketPrinterPaperOut,"小票打印机缺纸" },

           {KitErrCode.ErrRoomCardInit,"读写卡初始化失败" },
           {KitErrCode.ErrRoomCardMake,"写卡失败" },
           {KitErrCode.ErrRoomCardCopy,"复制卡失败" },
           {KitErrCode.ErrRoomCardRead,"读卡失败" },
           {KitErrCode.ErrRoomCardClear,"消除卡失败" },
           {KitErrCode.ErrRoomCardTimeOut,"读写卡超时" },
           {KitErrCode.ErrRoomCardWrite," 写卡失败" },

           {KitErrCode.ErrQRScannerConnectFailed,"二维码扫描器连接失败" },
           {KitErrCode.ErrQRScanInitFaild,"二维码扫描器初始化失败" },
           {KitErrCode.ErrQRScanTimeout,"二维码扫描器扫描超时" },
           {KitErrCode.ErrQRScanCanceled,"扫描已取消" },
           {KitErrCode.ErrQRScanExecuteFailed,"扫描命令执行失败" },
        };
        public static string CodeString(this KitErrCode code)
        {
            if (_kitCodeMsgs.TryGetValue(code, out string msg))
            {
                return msg;
            }
            return "Unknown Error";
        }

        public static  List<ErrDef> ListAll()
        {
            var ret = new List<ErrDef>();
            foreach(var kv in _kitCodeMsgs)
            {
                ret.Add(new ErrDef() { Code = (int)kv.Key, ErrorName = kv.Key.ToString(), ErrorDesc = kv.Value });
            }
            return ret;
        }
    }

    public class ErrDef
    {
        public int Code { get; set; }
        public string ErrorName { get; set; }
        public string ErrorDesc { get; set; }
    }
}
