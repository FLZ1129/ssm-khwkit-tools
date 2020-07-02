using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace khwkit_tools.Beans
{
    public enum KitEvents
    {
        /// <summary>
        /// 刷卡交易人为取消
        /// </summary>
        BANK_CARD_PAY_CANCLED_BY_HUMAN,
        /// <summary>
        /// 刷卡交易失败
        /// </summary>
        BANK_CARD_PAY_ERROR,
        /// <summary>
        /// 刷卡交易完成
        /// </summary>
        BANK_CARD_PAY_SUCESS,
        /// <summary>
        /// 银联刷卡初始化失败
        /// </summary>
        BANK_CARD_PAY_INIT_ERROR,
        /// <summary>
        /// 等待用户插入银行卡
        /// </summary>
        BANK_CARD_PAY_WANT_INPUT_CARD,
        /// <summary>
        /// 读卡失败
        /// </summary>
        BANK_CARD_PAY_READ_CARD_ERROR,
        /// <summary>
        /// 读卡成功
        /// </summary>
        BANK_CARD_PAY_READ_CARD_SUCCESS,
        /// <summary>
        /// 银联键盘开启失败
        /// </summary>
        BANK_CARD_PAY_START_PIN_ERROR,
        /// <summary>
        /// 等待用户输入密码
        /// </summary>
        BANK_CARD_PAY_WANT_INPUT_PASS,
        /// <summary>
        /// 银联键盘输入数字
        /// </summary>
        BANK_CARD_PAY_KEYBOARD_INPUT_PASS_KEY,
        /// <summary>
        /// 银联键盘输入删除
        /// </summary>
        BANK_CARD_PAY_KEYBOARD_INPUT_DEL,
        ///// <summary>
        ///// 银联键盘输入清除
        ///// </summary>
        //BANK_CARD_PAY_KEYBOARD_INPUT_CLEAR,
        /// <summary>
        /// 银联键盘输入取消
        /// </summary>
        BANK_CARD_PAY_KEYBOARD_INPUT_CANCEL,
        /// <summary>
        /// 银联键盘输入确认
        /// </summary>
        BANK_CARD_PAY_KEYBOARD_INPUT_CONFIRM,

        /// <summary>
        /// 银联键盘输入超时
        /// </summary>
        BANK_CARD_PAY_KEYBOARD_INPUT_TIMEOUT,
        /// <summary>
        /// 交易处理中
        /// </summary>
        BANK_CARD_PAY_TRANS_PROCESSING,

        UNKNOWN=1000
    }
    public class KitEventArg
    {
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("Event")]
        public string EventStr { get; set; }
        [JsonIgnore]
        public KitEvents Event => Str2Event(EventStr);
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }

        private static Dictionary<string, KitEvents> _eventMap=new Dictionary<string, KitEvents>() {
            { "BANK_CARD_PAY_CANCLED_BY_HUMAN",KitEvents.BANK_CARD_PAY_CANCLED_BY_HUMAN },
            { "BANK_CARD_PAY_ERROR",KitEvents.BANK_CARD_PAY_ERROR },
            { "BANK_CARD_PAY_SUCESS",KitEvents.BANK_CARD_PAY_SUCESS },
            { "BANK_CARD_PAY_INIT_ERROR",KitEvents.BANK_CARD_PAY_INIT_ERROR },
            { "BANK_CARD_PAY_WANT_INPUT_CARD",KitEvents.BANK_CARD_PAY_WANT_INPUT_CARD },
            { "BANK_CARD_PAY_READ_CARD_ERROR",KitEvents.BANK_CARD_PAY_READ_CARD_ERROR },
            { "BANK_CARD_PAY_READ_CARD_SUCCESS",KitEvents.BANK_CARD_PAY_READ_CARD_SUCCESS },
            { "BANK_CARD_PAY_START_PIN_ERROR",KitEvents.BANK_CARD_PAY_START_PIN_ERROR },
            { "BANK_CARD_PAY_WANT_INPUT_PASS",KitEvents.BANK_CARD_PAY_WANT_INPUT_PASS },
            { "BANK_CARD_PAY_KEYBOARD_INPUT_PASS_KEY",KitEvents.BANK_CARD_PAY_KEYBOARD_INPUT_PASS_KEY },
            { "BANK_CARD_PAY_KEYBOARD_INPUT_DEL",KitEvents.BANK_CARD_PAY_KEYBOARD_INPUT_DEL },
            { "BANK_CARD_PAY_KEYBOARD_INPUT_CANCEL",KitEvents.BANK_CARD_PAY_KEYBOARD_INPUT_CANCEL },
            { "BANK_CARD_PAY_KEYBOARD_INPUT_CONFIRM",KitEvents.BANK_CARD_PAY_KEYBOARD_INPUT_CONFIRM },
            { "BANK_CARD_PAY_KEYBOARD_INPUT_TIMEOUT",KitEvents.BANK_CARD_PAY_KEYBOARD_INPUT_TIMEOUT },
            { "BANK_CARD_PAY_TRANS_PROCESSING",KitEvents.BANK_CARD_PAY_TRANS_PROCESSING },
        };
        private static KitEvents Str2Event(string str)
        {
            if (str.IsEmpty() || !_eventMap.ContainsKey(str))
            {
                return KitEvents.UNKNOWN;
            }
            return _eventMap[str];
        }
    }
}
