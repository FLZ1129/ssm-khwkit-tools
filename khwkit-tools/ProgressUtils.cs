using System.Windows.Forms;
using CrazySharp.Base;

namespace CrazySharp.Std
{
    public static class ProgressUtils
    {
        public static ProgressBarOperator ShowProgress() {
            return ShowProgress("", false, false, 100, 0, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(ProgressBarStyle style) {
            return ShowProgress("", false, false, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(int max, int min = 0) {
            return ShowProgress("", false, false, max, min, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(int max, ProgressBarStyle style) {
            return ShowProgress("", false, false, max, 0, style);
        }

        public static ProgressBarOperator ShowProgress(string tip) {
            return ShowProgress(tip, true, false, 100, 0, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(string tip, ProgressBarStyle style) {
            return ShowProgress(tip, true, false, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(string tip, int max) {
            return ShowProgress(tip, true, false, max, 0, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(string tip, int max, ProgressBarStyle style) {
            return ShowProgress(tip, true, false, max, 0, style);
        }

        public static ProgressBarOperator ShowProgress(bool showPercent) {
            return ShowProgress("", false, showPercent, 100, 0, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(bool showPercent, ProgressBarStyle style) {
            return ShowProgress("", false, showPercent, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(int max, bool showPercent) {
            return ShowProgress("", false, showPercent, max, 0, ProgressBarStyle.Blocks);
        }

        public static ProgressBarOperator ShowProgress(string tip, bool showPercent) {
            return ShowProgress(tip, true, showPercent, 100, 0, ProgressBarStyle.Blocks);
        }

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="tip"></param>
        ///  <param name="showTip"></param>
        ///  <param name="showPercent"></param>
        ///  <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="style"></param>
        ///  <returns></returns>
        public static ProgressBarOperator ShowProgress(string tip, bool showTip, bool showPercent, int max, int min,
            ProgressBarStyle style) {
            return ShowProgress(null, tip, showTip, showPercent, max, min, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner) {
            return ShowProgress(owner, "", false, false, 100, 0, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, ProgressBarStyle style) {
            return ShowProgress(owner, "", false, false, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, int max, int min = 0) {
            return ShowProgress(owner, "", false, false, max, min, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, int max, ProgressBarStyle style) {
            return ShowProgress(owner, "", false, false, max, 0, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip) {
            return ShowProgress(owner, tip, true, false, 100, 0, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip, ProgressBarStyle style) {
            return ShowProgress(owner, tip, true, false, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip, int max) {
            return ShowProgress(owner, tip, true, false, max, 0, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip, int max, ProgressBarStyle style) {
            return ShowProgress(owner, tip, true, false, max, 0, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, bool showPercent) {
            return ShowProgress(owner, "", false, showPercent, 100, 0, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, bool showPercent, ProgressBarStyle style) {
            return ShowProgress(owner, "", false, showPercent, 100, 0, style);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, int max, bool showPercent) {
            return ShowProgress(owner, "", false, showPercent, max, 0, ProgressBarStyle.Marquee);
        }

        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip, bool showPercent) {
            return ShowProgress(owner, tip, true, showPercent, 100, 0, ProgressBarStyle.Marquee);
        }

        ///   <summary>
        /// 
        ///   </summary>
        /// <param name="owner"></param>
        /// <param name="tip"></param>
        ///   <param name="showTip"></param>
        ///   <param name="showPercent"></param>
        ///   <param name="max"></param>
        ///  <param name="min"></param>
        ///  <param name="style"></param>
        ///   <returns></returns>
        public static ProgressBarOperator ShowProgress(this BaseForm owner, string tip, bool showTip, bool showPercent, int max, int min,
            ProgressBarStyle style) {
            ProcessingForm progressBar =
                owner != null ? owner.GetProgressBarIns()
                : new ProcessingForm(null);
            progressBar.TipText = tip;
            progressBar.ShowTipText = showTip;
            progressBar.ShowPercentage = showPercent;
            progressBar.Max = max;
            progressBar.Min = min;
            progressBar.ProgressStyle = style;
            progressBar.Visible = true;
            progressBar.BringToFront();
            return new ProgressBarOperator(progressBar);
        }
    }
}