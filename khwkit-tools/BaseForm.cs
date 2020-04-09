using CrazySharp.Std;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CrazySharp.Base
{
    public enum FormStartMode
    {
        DEFAULT,
        MODEL
    }

    public class BaseForm : Form
    {
        [Flags]
        public enum KeyModifiers
        {
            NONE = 0x00,
            ALT = 0x01,
            CTRL = 0x02,
            SHIFT = 0x04,
            WINDOWS_KEY = 0x08
        }

        private bool hasRemoveFromMgr;

        //存储当前最大的热键Id
        private static int hotKeyIdIdx = 100;

        /// <summary>
        /// 自动生成热键Id
        /// </summary>
        private int AllocHotKyeId => Interlocked.Increment(ref hotKeyIdIdx);

        private readonly Dictionary<int, HotKeyHolder> hotKeysDict = new Dictionary<int, HotKeyHolder>();
        private readonly Once progressBarInsOnce = new Once();
        private ProcessingForm progressBarInsIns;

        public ProcessingForm GetProgressBarIns() {
            progressBarInsOnce.Do(() => {
                progressBarInsIns = new ProcessingForm(this);
            });
            return progressBarInsIns;
        }
        public BaseForm()
        {
            hasRemoveFromMgr = false;
            this.UseDefaultFormStyle();
            //
            //RegisterHotKey(Keys.Escape, Finish);
            Load += FormLoadDetails;
            Activated += FormResumeDetails;
            Deactivate += FormPauseDetails;
            FormClosed += FormClosedDetails;
        }

        private void FormLoadDetails(object sender, EventArgs e) {
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("宋体", 13f, FontStyle.Regular, GraphicsUnit.Pixel, 134);
            OnCreate();
        }

        private void FormClosedDetails(object sender, FormClosedEventArgs e)
        {
            OnDestroy();
            hotKeysDict.Clear();
        }

        /// <summary>
        /// Form加载完毕
        /// </summary>
        protected virtual void OnCreate()
        {
        }

        /// <summary>
        ///  Form 获取焦点
        /// </summary>
        protected virtual void OnResume()
        {
        }

        /// <summary>
        ///  Form 失去焦点
        /// </summary>
        protected virtual void OnPause()
        {
        }

        /// <summary>
        /// Form被关闭
        /// </summary>
        protected virtual void OnDestroy()
        {
            CheckClose(true);
        }

        public void DisableInteractive() {
            void Details() {
                Enabled = false;
            }

            if (InvokeRequired)
            {
                this.DispatchUi(Details);
            } else
            {
                Details();
            }
        }

        public void EnableInteractive() {
            void Details() {
                Enabled = true;
            }

            if (InvokeRequired)
            {
                this.DispatchUi(Details);
            } else
            {
                Details();
            }
        }

        public void Finish()
        {
            Close();
        }

        public void StartForm(FormStartMode startMode = FormStartMode.DEFAULT)
        {
            if (startMode == FormStartMode.DEFAULT) { Show(); } else { ShowDialog(); }
        }

        private void CheckClose(bool closeButtomClick)
        {
            if (hasRemoveFromMgr) return;
            hasRemoveFromMgr = true;
            if (!closeButtomClick)
            {
                Close();
            }
        }

        /// <summary>
        /// 窗体失去焦点时取消注册热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPauseDetails(object sender, EventArgs e)
        {
            foreach (var hk in hotKeysDict)
            {
                NativeMethods.UnregisterHotKey(Handle, hk.Key);
            }
            OnPause();
        }

        /// <summary>
        /// 窗体激活时注册热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormResumeDetails(object sender, EventArgs e)
        {
            OnResume();
            foreach (var hk in hotKeysDict)
            {
                if (hk.Value != null)
                {
                    NativeMethods.RegisterHotKey(Handle, hk.Key, (UInt32)hk.Value.Modifiers, hk.Value.Key);
                }
            }
        }

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyModifiers"></param>
        /// <param name="fun"></param>
        public void RegisterHotKey(Keys key, KeyModifiers keyModifiers, Action fun)
        {
            int id = AllocHotKyeId;
            //如果存在就重新申请
            while (hotKeysDict.ContainsKey(id))
            {
                id = AllocHotKyeId;
            }
            hotKeysDict.Add(id, new HotKeyHolder { Modifiers = keyModifiers, Key = key, Fun = fun });
        }

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="key"></param>
        public void RegisterHotKey(Keys key, Action fun)
        {
            RegisterHotKey(key, KeyModifiers.NONE, fun);
        }

        protected override void WndProc(ref Message m)
        {
            //按快捷键
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                if (hotKeysDict.ContainsKey(id))
                {
                    //响应快捷键
                    hotKeysDict[id]?.Fun?.Invoke();
                }
            }
            base.WndProc(ref m);
        }

        private class HotKeyHolder
        {
            public KeyModifiers Modifiers { get; set; }
            public Keys Key { get; set; }
            public Action Fun { get; set; }
        }

        private static class NativeMethods
        {
            /// <summary>
            /// 注册快捷键
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="id"></param>
            /// <param name="fsModifiers"></param>
            /// <param name="vk"></param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, Keys vk);

            /// <summary>
            /// 取消注册快捷键
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}