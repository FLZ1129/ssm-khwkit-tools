//尝试采用DataSource的方式管理DataGridView数据
#define NOT_DATA_GRID_VIEW_USE_DATASOURCE
using CrazySharp.Base.Attributes;
using CrazySharp.Base.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using NLog;

namespace CrazySharp.Std
{
    public static class Extensions
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #region string 扩展

        /// <summary>
        /// 转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDbc(this string input) {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            char[] c = input.Trim().ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }

                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }

            return new string(c);
        }

        private static byte[] GetMd5Hash(this byte[] input) {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] md5Bytes = md5Hash.ComputeHash(input);
                return md5Bytes;
            }
        }

        /// <summary>
        /// 计算MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public static string ComputeMd5(this string input) {
            return ComputeMd5(Encoding.UTF8.GetBytes(input));
        }

        public static string ComputeMd5(this byte[] input) {
            StringBuilder sb = new StringBuilder();

            byte[] md5Bytes = GetMd5Hash(input);
            for (int i = 0; i < md5Bytes.Length; i++)
            {
                sb.Append(md5Bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 校验MD5
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMd5Hash(this byte[] input, string hash) {
            string hashOfInput = ComputeMd5(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(hashOfInput, hash);
        }

        public static bool VerifyMd5Hash(this string input, string hash) {
            return VerifyMd5Hash(Encoding.UTF8.GetBytes(input), hash);
        }

        public static string ToJsonString(this object obj, Formatting formatting = Formatting.None) {
            if (obj == null)
            {
                return "{}";
            }
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public static JsonObject ToJsonObject(this object obj) {
            var ret = new JsonObject();
            if (obj == null)
            {
                return ret;
            }
            JObject jobj = JObject.Parse(obj.ToJsonString());
            foreach (var kv in jobj)
            {
                ret.Add(kv.Key, kv.Value);
            }
            return ret;
        }

        public static string ToJsonString(this string obj) {
            return obj;
        }

        public static T ParseJson<T>(this string obj) {
            //需要在这里统一取出转义字符
            return JsonConvert.DeserializeObject<T>(obj);
        }

        /// <summary>
        /// 编码Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeToBase64(this string str) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 编码Base64数组
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static string EncodeToBase64(this byte[] bs) {
            return Convert.ToBase64String(bs);
        }

        public static bool TrySaveToFile(this string contents,string file)
        {
            try
            {
                File.WriteAllText(file,contents);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"TrySaveToFile Error : {ex.Message}");
                return false;
            }
        }

        public static string TryReadAllText(this string file)
        {
            try
            {
                return File.ReadAllText(file);
            }
            catch (Exception ex)
            {
                logger.Error($"TrySaveToFile Error : {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// 解码Base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeBase64String(this string str) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        public static byte[] GetBytes(this string str, Encoding encode) {
            return encode.GetBytes(str);
        }

        public static byte[] GetBytes(this string str) {
            return GetBytes(str, Encoding.UTF8);
        }
        public static string GetU8String(this byte[] data) {
            return GetString(data, Encoding.UTF8);
        }
        public static string GetString(this byte[] data, Encoding encode)
        {
            return encode.GetString(data);
        }

        public static bool IsEmail(this string email)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                MailAddress m = new MailAddress(email);
                return true;
            } catch (FormatException)
            {
                return false;
            }
        }

        #endregion string 扩展

        #region Bean相关扩展

        /// <summary>
        /// 深复制对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Copy<T>(this T obj) where T : class, ICopyable {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return formatter.Deserialize(stream) as T;
            }
        }

        /// <summary>
        /// 获取表头设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<DgvColAttribute> GetDgvHeaders<T>() where T : class {
            List<DgvColAttribute> colList = new List<DgvColAttribute>();
            var propertyInfos = Utils.GetDgvColAttrPropertyInfos<T>();
            foreach (var propertyInfo in propertyInfos)
            {
                colList.Add(propertyInfo.AttributeData);
            }

            return new List<DgvColAttribute>(colList.OrderBy(it => it.Index).ToArray());
        }

        public static List<DataTableColumnAttribute> GetDataTableHeaders<T>() where T : class
        {
            List<DataTableColumnAttribute> colList = new List<DataTableColumnAttribute>();
            foreach (var propertyInfo in Utils.GetDataColumnAttrPropertyInfos < T > ())
            {
                colList.Add(propertyInfo.AttributeData);
            }
            return new List<DataTableColumnAttribute>(colList.OrderBy(it => it.ColumnIndex).ToArray());
        }

        public static object[] ToDgvRow<T>(this T item) {
            List<object> ret = new List<object>();
            var propertyInfos = Utils.GetDgvColAttrPropertyInfos<T>();
            foreach (var propertyInfo in propertyInfos)
            {
                //取值
                ret.Add(propertyInfo.GetValue(item));
            }

            return ret.ToArray();
        }

        #endregion Bean相关扩展

        #region DataGridView 扩展

        /// <summary>
        /// 设置DataGridView表头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="setStyle"></param>
        public static void UseHeader<T>(this DataGridView dgv, bool setStyle = true) where T : class {
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = false;
            var colPropertyInfos = Utils.GetDgvColAttrPropertyInfos<T>().OrderBy(it=>it.AttributeData.Index).ToList();
            int columnCount = colPropertyInfos.Count;
            if (!colPropertyInfos.Any()) return;
            dgv.ColumnCount = columnCount;
            for (int i = 0; i < columnCount; ++i)
            {
                var colProAttr = colPropertyInfos[i].AttributeData;
                dgv.Columns[i].Name = colProAttr.Name;
#if DATA_GRID_VIEW_USE_DATASOURCE
                var colPro = colPropertyInfos[i];
                dgv.Columns[i].DataPropertyName = colPro.PropertyName;
#endif
                dgv.Columns[i].ReadOnly = colProAttr.ReadOnly;
                dgv.Columns[i].AutoSizeMode = colProAttr.Mode;
                if (!Utils.IsEmpty(colProAttr.Format))
                {
                    dgv.Columns[i].DefaultCellStyle.Format = colProAttr.Format;
                }
                if (!setStyle || colProAttr.ReadOnly) continue;
                //字体加粗
                var font = dgv.Font;
                dgv.Columns[i].DefaultCellStyle.Font = new Font(font, FontStyle.Bold);
            }
        }

        /// <summary>
        /// 清空所有行
        /// </summary>
        /// <param name="dgv"></param>
        public static void Clear(this DataGridView dgv) {
            Debug.Assert(dgv != null);
#if DATA_GRID_VIEW_USE_DATASOURCE
            dgv.DataSource = null;
#else
           dgv.Rows.Clear();
#endif
        }

        /// <summary>
        /// 向DataGridView中新增多行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="list"></param>
        public static void AddRows<T>(this DataGridView dgv, IEnumerable<T> list)
            where T : class {
            Debug.Assert(dgv != null);
            dgv.Clear();
            if (list != null)
            {
#if DATA_GRID_VIEW_USE_DATASOURCE
                    dgv.DataSource = list;
#else
                foreach (var item in list)
                {
                    dgv.AddRow(item);
                }
#endif
            }
            dgv.ClearSelection();
        }

        public static void InsertRow<T>(this DataGridView dgv, int index, T item) where T : class {
            Debug.Assert(dgv != null);
            Debug.Assert(index <= dgv.RowCount);
            if (item == null)
            {
                return;
            }
            void DoInsert() {
#if DATA_GRID_VIEW_USE_DATASOURCE
                var ds = (dgv.DataSource as IEnumerable<T>)?. ToList()?? new List<T>() { item };
                dgv.DataSource = ds;
#else
                dgv.Rows.Insert(index, item.ToDgvRow());
                dgv.Rows[index].Tag = item;
#endif
            }
            if (dgv.InvokeRequired) { dgv.Invoke((Action)DoInsert); } else { DoInsert(); }
        }

        /// <summary>
        /// 向DataGridView中新增一行
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="item"></param>
        /// <returns>返回行索引</returns>
        public static int AddRow<T>(this DataGridView dgv, T item) where T : class {
            Debug.Assert(dgv != null);
            if (item == null)
            {
                return -1;
            }
            int idx = -1;

            void DoAdd() {
#if DATA_GRID_VIEW_USE_DATASOURCE
                var ds = (dgv.DataSource as IEnumerable<T>)?. ToList()?? new List<T>() { item };
                dgv.DataSource = ds;
                idx = dgv.RowCount-1;
#else
                idx = dgv.Rows.Add(item.ToDgvRow());
                dgv.Rows[idx].Tag = item;
#endif
            }

            if (dgv.InvokeRequired) { dgv.Invoke((Action)DoAdd); } else { DoAdd(); }
            return idx;
        }

        /// <summary>
        /// 新增一行并且将其状态设置为选中
        /// 参数意义参见 int AddRow(...)
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="item"></param>
        public static int AddRowAndSelect<T>(this DataGridView dgv, T item) where T : class {
            int rowIdx = dgv.AddRow(item);
            dgv.ClearSelection();
            dgv.SelectRowAt(rowIdx);
            return rowIdx;
        }

        /// <summary>
        /// 尝试更新一行
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="item"></param>
        /// <param name="strict">如果true,则失败会抛出断言</param>
        /// <returns></returns>
        public static int TryUpdateRow<T>(this DataGridView dgv, T item, bool strict = false) where T : class {
            int idx = -1;
            foreach (DataGridViewRow itemRow in dgv.Rows)
            {
                if (itemRow.Tag == item)
                {
                    idx = itemRow.Index;
                    break;
                }
            }
            if (strict)
            {
                Debug.Assert(dgv != null && idx >= 0 && idx < dgv.RowCount);
            }
            try
            {
                if (idx != -1) { return UpdateRow(dgv, idx, item); }
            } catch
            {
                //快速点击搜索主机时，因为是异步执行的，可能造成多线程同时操作Datagridview，
                //这地方用try..catch是最省事的办法
            }
            return -1;
        }

        /// <summary>
        /// 更新行
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int UpdateRow<T>(this DataGridView dgv, T item) where T : class {
            return dgv.TryUpdateRow(item, true);
        }

        /// <summary>
        /// 更新行
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int UpdateRow<T>(this DataGridView dgv, int index, T item) where T : class {
            void DoUpdate() {
#if DATA_GRID_VIEW_USE_DATASOURCE
                   dgv.InvalidateRow(index);
#else
                object[] updateValues = item.ToDgvRow();
                int count = Math.Min(updateValues.Length, dgv.ColumnCount);
                DataGridViewRow updateTargetRow = dgv.Rows[index];
                updateTargetRow.Tag = item;
                for (int i = 0; i < count; ++i)
                {
                    updateTargetRow.Cells[i].Value = updateValues[i];
                }
#endif
            }
            if (dgv.InvokeRequired) { dgv.Invoke((Action)DoUpdate); } else { DoUpdate(); }
            return index;
        }

        /// <summary>
        /// 获取tagData并且将tag转换为指定类型,如果失败,将抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T GetTagDataAs<T>(this DataGridViewRow row) where T : class
        {
            //return row.DataBoundItem as T;
#if DATA_GRID_VIEW_USE_DATASOURCE
            return row.DataBoundItem as T;
#else
             return row.Tag as T;
#endif
        }

        /// <summary>
        /// 获取指定行的Tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetTagAt<T>(this DataGridView dgv, int index) where T : class {
            Debug.Assert(dgv != null && index >= 0 && index < dgv.RowCount);
            return dgv.Rows[index].GetTagDataAs<T>();
        }

        /// <summary>
        /// 尝试获取指定行的Tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryGetTagAt<T>(this DataGridView dgv, int index, out T data) where T : class {
            data = null;
            //Tester-Doer模式
            if (dgv != null && index >= 0 && index < dgv.RowCount)
            {
                data = dgv.GetTagAt<T>(index);
            }

            return data != null;
        }

        /// <summary>
        /// 包装了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="index"></param>
        /// <param name="data"></param>
        public static void GetTagAt<T>(this DataGridView dgv, int index, out T data) where T : class {
            // ReSharper disable once RedundantAssignment
            bool ok = dgv.TryGetTagAt(index, out data);
            Debug.Assert(ok);
        }

        /// <summary>
        /// 获取第一个选中行的Tag的数据和索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetTagAtFirstSelectedRow<T>(this DataGridView dgv, out T data, out int index)
            where T : class {
            data = null;
            index = -1;
            if (dgv.Rows.Count == 0 || dgv.SelectedRows.Count == 0)
            {
                return false;
            }

            index = dgv.SelectedRows[0].Index;
            data = dgv.GetTagAt<T>(index);
            return data != null;
        }

        /// <summary>
        /// 获取第一个选中行的Tag的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        public static bool TryGetTagAtFirstSelectedRowData<T>(this DataGridView dgv, out T data)
            where T : class {
            return dgv.TryGetTagAtFirstSelectedRow(out data, out _);
        }

        public static bool TryGetAllSelectedRowTag<T>(this DataGridView dgv, out List<T> data) where T : class {
            data = new List<T>();
            var list = new List<T>();
            dgv.ForeachSelectedRows<T>(it => {
                list.Add(it);
            });
            data = list;
            return data.Count > 0;
        }

        /// <summary>
        /// 遍历所有选择行并执行操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="func"></param>
        public static void ForeachSelectedRows<T>(this DataGridView dgv, Action<T> func) where T : class {
            if (dgv.SelectedRows.Count == 0)
            {
                return;
            }

            bool FuncWrapper(int _, T item) {
                func?.Invoke(item);
                return true;
            }
            ForeachRowCollection<T>(dgv.SelectedRows, FuncWrapper);
        }

        /// <summary>
        /// 迭代选中的行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dg"></param>
        /// <param name="cb">cb的参数为Row的tag,cb返回false则停止迭代</param>
        public static void ForeachSelectedRows<T>(this DataGridView dg, Func<int, T, bool> cb) where T : class {
            if (cb == null)
            {
                return;
            }

            ForeachRowCollection(dg.SelectedRows, cb);
        }

        /// <summary>
        /// 迭代所有行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dg"></param>
        /// <param name="cb"></param>
        public static void ForeachRows<T>(this DataGridView dg, Func<int, T, bool> cb) where T : class {
            if (cb == null)
            {
                return;
            }

            ForeachRowCollection(dg.Rows, cb);
        }

        public static void Filter<T>(this DataGridView dg, Func<int, T, bool> cb) where T : class {
            if (cb == null)
            {
                return;
            }
            foreach (DataGridViewRow row in dg.Rows)
            {
                row.Visible = cb.Invoke(row.Index, row.GetTagDataAs<T>());
            }
        }

        public static void ForeachRowCollection<T>(IEnumerable rowCollection, Func<int, T, bool> cb) where T : class {
            if (cb == null)
            {
                return;
            }

            foreach (DataGridViewRow row in rowCollection)
            {
                if (!cb.Invoke(row.Index, row.GetTagDataAs<T>()))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 移除所有选择行并执行操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dgv"></param>
        /// <param name="func"></param>
        public static void RemoveSelectedRows<T>(this DataGridView dgv, Action<T> func) where T : class {
            if (dgv == null || dgv.SelectedRows.Count == 0) { return; }
            bool FuncWrapper(int _, T item) {
                func?.Invoke(item);
                return true;
            }
            dgv.RemoveSelectedRows<T>(FuncWrapper);
        }

        public static void RemoveSelectedRows<T>(this DataGridView dgv, Func<int, T, bool> func) where T : class
        {
#if DATA_GRID_VIEW_USE_DATASOURCE
            var ds = (dgv.DataSource as IEnumerable<T>)?.ToList() ?? new List<T>();
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                ds.Remove(row.GetTagDataAs<T>());
            }
            dgv.DataSource = ds;
#else
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (!func.Invoke(row.Index, row.GetTagDataAs<T>()))
                {
                    break;
                }
                if (dgv.InvokeRequired)
                {
                    dgv.DispatchUi(() => { dgv.Rows.Remove(row); });
                }
                else
                {
                    dgv.Rows.Remove(row);
                }
            }
#endif

        }

        /// <summary>
        /// 选择指定行
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="rowIdx"></param>
        public static void SelectRowAt(this DataGridView dg, int rowIdx) {
            Debug.Assert(rowIdx > -1 && dg.RowCount > rowIdx);
            dg.ClearSelection();
            dg.Rows[rowIdx].Selected = true;
        }

        /// <summary>
        /// 双击cell编辑功能,cb返回true:可编辑,false:不可编辑
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="cb"></param>
        public static void InstallCellEdit(this DataGridView dg, Func<int, int, bool> cb) {
            Debug.Assert(cb != null);
            dg.MouseDown += (sender, e) => {
                dg.GetClickedRowColIndex(e.X, e.Y, out int hitRowIdx, out int hitColIdx);
                if (hitRowIdx == -1
                    || hitColIdx == -1
                    || !dg.GetCellAt(hitRowIdx, hitColIdx).IsInEditMode)
                {
                    dg.EndEdit();
                }
            };

            dg.CellDoubleClick += (sender, args) => {
                int rowIdx = args.RowIndex;
                int colIdx = args.ColumnIndex;
                if (rowIdx == -1 || colIdx == -1) return;
                var cell = dg.GetCellAt(rowIdx, colIdx);
                if (cell.IsInEditMode || !cb(rowIdx, colIdx)) return;
                dg.CurrentCell = cell;
                cell.ReadOnly = false;
                dg.BeginEdit(true);
            };

            dg.CellEndEdit += (sender, args) => {
                dg.GetCellAt(args.RowIndex, args.ColumnIndex).ReadOnly = true;
            };
        }

        public static void InstallCellEndEdit<T>(this DataGridView dg, int columnIndex, Func<int, string, T, bool> cb) where T : class {
            Debug.Assert(cb != null);
            dg.CellEndEdit += (sender, args) => {
                int rowIdx = args.RowIndex;
                int colIdx = args.ColumnIndex;
                if (colIdx != columnIndex) { return; }
                DataGridViewCell cell = dg.GetCellAt(rowIdx, colIdx);
                string newCellValue = cell.Value?.ToString().Trim().ToDbc() ?? string.Empty;
                dg.GetTagAt(rowIdx, out T data);
                if (cb.Invoke(rowIdx, newCellValue, data))
                {
                    //  Utils.Info("修改成功");
                }
                dg.UpdateRow(rowIdx, data);
            };
        }

        /// <summary>
        /// 获取指定位置的cell
        /// </summary>
        /// <param name="dg">datagridview</param>
        /// <param name="rowIdx">row index</param>
        /// <param name="colIdx">col index</param>
        /// <returns></returns>
        public static DataGridViewCell GetCellAt(this DataGridView dg, int rowIdx, int colIdx) {
            var cell = dg.Rows[rowIdx].Cells[colIdx];
            Debug.Assert(cell != null);
            return cell;
        }

        /// <summary>
        /// 获取点击的RowIndex
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rowIdx"></param>
        /// <param name="colIdx"></param>
        /// <returns></returns>
        public static void GetClickedRowColIndex(this DataGridView dg, int x, int y, out int rowIdx, out int colIdx) {
            rowIdx = -1;
            colIdx = -1;
            var hitInfo = dg.HitTest(x, y);
            if (hitInfo.Type != DataGridViewHitTestType.Cell)
            {
                return;
            }
            rowIdx = hitInfo.RowIndex;
            colIdx = hitInfo.ColumnIndex;
        }
        /// <summary>
        /// 禁用dataGridView的多选
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="enable"></param>
        public static void SetMultiSelect(this DataGridView dg, bool enable)
        {
            dg.MultiSelect = enable;
        }

        public static void SetCopySingleCellMode(this DataGridView dg)
        {
            dg.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            //支持复制单个单元格
            dg.KeyDown += (s, e) => {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    var cell = dg.CurrentCell;
                    string cellText = cell?.Value?.ToString().Trim() ?? "";
                    if (!Utils.IsEmpty(cellText)) { Clipboard.SetText(cellText); }
                }
            };
        }
        /// <summary>
        /// 获取第一个选中的行
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        public static int FirstSelectRow(this DataGridView dg)
        {
            if (dg.SelectedRows.Count < 1)
            {
                return -1;
            }

            return dg.SelectedRows[0].Index;
        }


        #endregion DataGridView 扩展

        #region DataTable 扩展

        public static void UseHeader<T>(this DataTable dt) where T : class
        {
            dt.Columns.Clear();
            List<DataTableColumnAttribute> colList = GetDataTableHeaders<T>();
            if (colList == null || !colList.Any()) return;
            foreach (var t in colList)
            {
                dt.Columns.Add(t.Name, typeof(string));
            }
        }

        public static object[] ToDataTableRow<T>(this T item)
        {
            List<object> ret = new List<object>();
            var propertyInfos = Utils.GetDataColumnAttrPropertyInfos<T>();
            foreach (var propertyInfo in propertyInfos)
            {
                //取值
                ret.Add(propertyInfo.GetValue(item));
            }

            return ret.ToArray();
        }

        public static DataRow AddRow<T>(this DataTable dt, T item) where T : class
        {
            Debug.Assert(dt != null);
            if (item == null)
            {
                return null;
            }
            return dt.Rows.Add(item.ToDataTableRow());
        }
        public static void AddRows<T>(this DataTable dt, IEnumerable<T> list)
            where T : class
        {
            Debug.Assert(dt != null);
            lock (dt)
            {
                dt.Clear();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        dt.AddRow(item);
                    }
                }
            }
        }
        #endregion

        public static void DispatchUi(this Control c, Action func) {
            try
            {
                if (c.InvokeRequired)
                {
                    c.Invoke(func);
                } else
                {
                    func.Invoke();
                }
            } catch
            {
                // ignored
            }
        }
        public static T DispatchUi<T>(this Control c, Func<T> func) {
            try
            {
                if (c.InvokeRequired)
                {
                    return (T)c.Invoke(func);
                } else
                {
                    return func.Invoke();
                }
            }
            catch
            {
                // ignored
            }

            return default(T);
        }

        public static byte[] GetBytes(this Icon icon) {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 截取数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Sub(this byte[] data, int index, int length) {
            byte[] ret = new byte[length];
            Array.Copy(data, index, ret, 0, length);
            return ret;
        }
    }
}