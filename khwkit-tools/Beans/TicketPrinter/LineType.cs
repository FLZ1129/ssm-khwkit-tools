namespace khwkit.Beans.TicketPrinter
{
    public enum LineType    {
        /// <summary>
        // 1. 标题 居中对齐，正常字体两倍
        /// </summary>
        TITLE = 1,
        /// <summary>
        /// 2. 小标题 居中对齐，正常字体
        /// </summary>
        SUBTITLE = 2,
        /// <summary>
        /// 3. 内容 正常字体
        /// </summary>
        BODY = 3,
        /// <summary>
        /// 4. 二维码 如开票码、微信公众号等
        /// </summary>
        QR_CODE = 4,
        /// <summary>
        /// 5. 备注 正常字体，左对齐
        /// </summary>
        REMARK = 5,
        /// <summary>
        /// 6. 分割线 一条细线
        /// </summary>
        SEP_LINE = 6,
        /// <summary>
        /// 7. 图片
        /// </summary>
        PIC = 7,
        /// <summary>
        /// 空白行
        /// </summary>
        BALNK=8,
    }
}
