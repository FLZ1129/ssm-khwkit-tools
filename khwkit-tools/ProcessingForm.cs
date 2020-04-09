using CrazySharp.Std;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CrazySharp.Base
{
    public partial class ProcessingForm : Form
    {
        private bool isMouseDown;
        private Point currentFormLocation; //当前窗体位置
        private Point currentMouseOffset; //当前鼠标的按下位置
        private int percent;
        private int current;
        private readonly BaseForm ownerForm;
        private bool ShowTitle => ShowPercentage || ShowTipText;

        private int Value {
            get => current;
            set { current = value; RefreshProgress(); }
        }

        public ProgressBarStyle ProgressStyle {
            get => pBar.Style;
            set => pBar.Style = value;
        }

        public bool ShowPercentage { get; set; }

        public bool ShowTipText { get; set; }

        public string TipText { get; set; }

        /// <summary>
        /// 进度条最大值
        /// </summary>
        public int Max { get; set; } = 100;

        /// <summary>
        /// 进度条最小值
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Min { get; set; }

        public ProcessingForm(BaseForm owner) {
            Owner = owner;
            ownerForm = owner;
            Init();
        }

        private void Init() {
            InitializeComponent();
            MouseDown += ProcessingFormOnMouseDown;
            MouseMove += ProcessingFormOnMouseMove;
            MouseUp += ProcessingFormOnMouseUp;
            Load += ProcessingFormOnLoad;
            VisibleChanged += ProcessingFormOnVisibleChanged;
        }

        private void ProcessingFormOnVisibleChanged(object sender, EventArgs e) {
            if (Visible)
            {
                pBar.Value = 0;
                ownerForm?.DisableInteractive();
                TopMost = ownerForm==null;
                Enabled = true;
                RefreshProgress();
                if (ownerForm != null)
                {
                    //计算位置,显示在父窗体中央
                    int x = ownerForm.Location.X + ownerForm.Width / 2 - Width / 2;
                    int y = ownerForm.Location.Y + ownerForm.Height / 2 - Height / 2;
                    Location = new Point(x, y);
                }
            } else
            {
                ownerForm?.EnableInteractive();
                TopMost = false;
                Enabled = false;
                if (ownerForm == null)
                {
                    Close();
                }
            }
        }

        private void ProcessingFormOnLoad(object sender, EventArgs e) {
            Value = 0;
            ResizeForm();
            RefreshTip();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value) {
            Value = value;
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <param name="step"></param>
        public void IncreaseValue(int step = 1) {
            Debug.Assert(step >= 0);
            Value += step;
        }

        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="tip"></param>
        public void SetTip(string tip) {
            TipText = tip;
            if (!string.IsNullOrWhiteSpace(TipText))
            {
                ShowTipText = true;
                ResizeForm();
                RefreshTip();
            }
        }

        public void Done() {
            this.DispatchUi(() => {
                Visible = false;
            });
        }

        private void ResizeForm() {
            Invoke((Action)delegate { Height = ShowTitle ? 70 : 35; });
        }

        private void RefreshProgress() {
            Debug.Assert(Value >= 0 && Value <= Max);
            percent = (int)Math.Ceiling((double)Value / Max * 100);
            Invoke((Action)delegate {
                pBar.Value = Value;
                RefreshTip();
            });
        }

        private void RefreshTip() {
            Invoke((Action)delegate {
                StringBuilder sb = new StringBuilder();
                if (ShowTipText) { sb.Append(TipText); }
                if (ShowPercentage) { sb.Append($"{percent} %"); }
                lbTitle.Text = sb.ToString();
            });
        }

        #region 支持拖动位置

        private void ProcessingFormOnMouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                currentFormLocation = Location;
                currentMouseOffset = MousePosition;
            }
        }

        private void ProcessingFormOnMouseUp(object sender, MouseEventArgs e) {
            isMouseDown = false;
        }

        private void ProcessingFormOnMouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                Point pt = MousePosition;
                var rangeX = currentMouseOffset.X - pt.X; //计算当前鼠标光标的位移，让窗体进行相同大小的位移
                var rangeY = currentMouseOffset.Y - pt.Y; //计算当前鼠标光标的位移，让窗体进行相同大小的位移
                Location = new Point(currentFormLocation.X - rangeX, currentFormLocation.Y - rangeY);
            }
        }

        #endregion 支持拖动位置
    }

    public class ProgressBarOperator
    {
        private readonly ProcessingForm pform;

        public ProgressBarOperator(ProcessingForm form) {
            Debug.Assert(form != null);
            pform = form;
        }

        /// <summary>
        /// 更改样式
        /// </summary>
        /// <param name="showTip"></param>
        /// <param name="showPercentage"></param>
        public void ChangeStyle(bool showTip, bool showPercentage) {
            pform.ShowTipText = showTip;
            pform.ShowPercentage = showPercentage;
        }

        public void ChangeStyle(bool showPercentage) {
            pform.ShowPercentage = showPercentage;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value) {
            pform.SetValue(value);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <param name="step"></param>
        public void IncreaseValue(int step = 1) {
            pform.IncreaseValue(step);
        }

        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="tip"></param>
        public void SetTip(string tip) {
            pform.SetTip(tip);
        }

        /// <summary>
        /// 结束进度条显示
        /// </summary>
        public void Done() {
            pform.Done();
        }
    }
}