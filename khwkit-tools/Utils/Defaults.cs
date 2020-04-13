using System;
using CrazySharp.Base;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CrazySharp.Std
{
    public static class Defaults
    {
        public static void UseDefaultJsonSetting()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 忽略循环引用
                setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //使用驼峰
                setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //忽略空值
                setting.NullValueHandling = NullValueHandling.Ignore;
                return setting;
            });
        }
        /// <summary>
        /// 默认的窗体样式
        /// </summary>
        /// <param name="form"></param>
        public static void UseDefaultFormStyle(this Form form) {
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public static void UseDefaultFormDialogStyle(this BaseForm form) {
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            //form.AutoSize = true;
            //form.ShowIcon = false;
            form.MaximizeBox = false;
            form.TopMost = true;
            form.MinimizeBox = false;
            form.RegisterHotKey(Keys.Escape, form.Finish);
        }

        /// <summary>
        /// 默认的表格样式
        /// </summary>
        /// <param name="dg"></param>
        public static void UseDefaultGridViewStyle(this DataGridView dg) {
            dg.ReadOnly = false;
            dg.EditMode = DataGridViewEditMode.EditProgrammatically;
            //dg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //dg.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
            dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.RowsDefaultCellStyle.Padding =new Padding(3,0,3,0);
            dg.BackgroundColor = Color.White;
            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg.AllowUserToAddRows = false;
            dg.AllowUserToResizeRows = false;
            dg.AllowUserToResizeColumns = true;
            dg.AllowUserToDeleteRows = false;
            dg.RowHeadersVisible = false;
            dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dg.SetCopySingleCellMode();
            dg.CellFormatting += (sender, e) =>
            {
                if (e.Value is DateTime)
                {
                    DateTime value = (DateTime)e.Value;
                    switch (value.Kind)
                    {
                        case DateTimeKind.Local:
                            break;
                        case DateTimeKind.Unspecified:
                            e.Value = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();
                            break;
                        case DateTimeKind.Utc:
                            e.Value = value.ToLocalTime();
                            break;
                    }
                }
            };
            //dg.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            ////支持复制单个单元格
            //dg.KeyDown += (s, e) => {
            //    if (e.Control && e.KeyCode == Keys.C)
            //    {
            //        var cell = dg.CurrentCell;
            //        string cellText = cell?.Value?.ToString().Trim() ?? "";
            //        if (!Utils.IsEmpty(cellText)) { Clipboard.SetText(cellText); }
            //    }
            //};
        }
    }
}