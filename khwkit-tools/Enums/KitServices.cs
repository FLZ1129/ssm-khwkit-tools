using System;

namespace khwkit.Enums
{
    [Flags]
    public enum KitServices
    {
        CardBox,
        IDReader,
        RoomCard,
        PSB,
        Printer,
        QRScanner,
        System,
        PayPi,
    }

    public static class KitServicesExtension
    {
        public static string ApiPathStr(this KitServices services)
        {
            switch (services)
            {
                case KitServices.CardBox:return "card_box";
                case KitServices.IDReader: return "id_reader";
                case KitServices.RoomCard: return "roomcard";
                case KitServices.PSB: return "psb";
                case KitServices.Printer: return "printer";
                case KitServices.QRScanner: return "qr_scanner";
                case KitServices.System: return "system";
                case KitServices.PayPi: return "pay_pi";
            }
            return "";
        }
        public static string FriendlyName(this KitServices services)
        {
            switch (services)
            {
                case KitServices.CardBox: return "收发卡机服务";
                case KitServices.IDReader: return "身份证采集服务";
                case KitServices.RoomCard: return "房卡读写服务";
                case KitServices.PSB: return "旅业服务";
                case KitServices.Printer: return "小票打印服务";
                case KitServices.QRScanner: return "二维码扫描服务";
                case KitServices.System: return "系统服务";
                case KitServices.PayPi: return "刷卡支付服务";
            }
            return "";
        }
        public static string FriendlyNameEn(this KitServices services)
        {
            switch (services)
            {
                case KitServices.CardBox: return "CardBox";
                case KitServices.IDReader: return "IDReader";
                case KitServices.RoomCard: return "RoomCard";
                case KitServices.PSB: return "PSB";
                case KitServices.Printer: return "Printer";
                case KitServices.QRScanner: return "QRScanner";
                case KitServices.System: return "System";
            }
            return "";
        }
    }
}
