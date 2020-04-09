
namespace khwkit.Beans.TicketPrinter
{
    public class TicketStyledItem
    {

        /**
         * 1. 标题 居中对齐，正常字体两倍
         * 2. 小标题 居中对齐，正常字体
         * 3. 内容 正常字体
         * 4. 二维码 如开票码、微信公众号等
         * 5. 备注 正常字体，左对齐
         * 6. 分割线 一条细线
         * 7. 图片
         * 8. 空白行
         */
        public LineType Type { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }
}
