using System;
using System.Windows.Forms;

namespace CrazySharp.Base.Attributes
{
    /// <summary>
    /// 标识类可与DataGridView绑定
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DataGridBindAbleAttribute : Attribute
    {
    }

    /// <summary>
    /// 标识字段可用于显示在DataGridView中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DgvColAttribute : Attribute
    {
        private const DataGridViewAutoSizeColumnMode
            DEFAULT_AUTO_SIZE_COLUMN_MODE = DataGridViewAutoSizeColumnMode.Fill;
        public string Name { get; }
        public int Index { get; }

        public bool ReadOnly { get; }
        public string Format { get; }

        public DataGridViewAutoSizeColumnMode Mode { get; }

        public DgvColAttribute(string colName)
            : this(colName, -1, true, DEFAULT_AUTO_SIZE_COLUMN_MODE, "") {
        }

        public DgvColAttribute(string colName, string format)
            : this(colName, -1, true, DEFAULT_AUTO_SIZE_COLUMN_MODE, format) {
        }

        public DgvColAttribute(string colName, DataGridViewAutoSizeColumnMode mode)
            : this(colName, -1, true, mode, "") {
        }

        public DgvColAttribute(string colName, DataGridViewAutoSizeColumnMode mode, string format)
            : this(colName, -1, true, mode, format) {
        }

        public DgvColAttribute(string colName, bool isReadonly)
            : this(colName, -1, isReadonly, DEFAULT_AUTO_SIZE_COLUMN_MODE, "") {
        }

        public DgvColAttribute(string colName, bool isReadonly, string format)
            : this(colName, -1, isReadonly, DEFAULT_AUTO_SIZE_COLUMN_MODE, format) {
        }

        public DgvColAttribute(string colName, bool isReadonly, DataGridViewAutoSizeColumnMode mode)
            : this(colName, -1, isReadonly, mode, "") {
        }

        public DgvColAttribute(string colName, bool isReadonly, DataGridViewAutoSizeColumnMode mode, string format)
            : this(colName, -1, isReadonly, mode, format) {
        }

        public DgvColAttribute(string colName, int index)
            : this(colName, index, true, DEFAULT_AUTO_SIZE_COLUMN_MODE, "") {
        }

        public DgvColAttribute(string colName, int index, string format)
            : this(colName, index, true, DEFAULT_AUTO_SIZE_COLUMN_MODE, format) {
        }

        public DgvColAttribute(string colName, int index, DataGridViewAutoSizeColumnMode mode)
           : this(colName, index, true, mode, "") {
        }

        public DgvColAttribute(string colName, int index, DataGridViewAutoSizeColumnMode mode, string format)
            : this(colName, index, true, mode, format) {
        }

        public DgvColAttribute(string colName, int index, bool isReadonly)
            : this(colName, index, isReadonly, DEFAULT_AUTO_SIZE_COLUMN_MODE, "") {
        }
        public DgvColAttribute(string colName, int index, bool isReadonly, DataGridViewAutoSizeColumnMode mode)
            : this(colName, index, isReadonly, mode, "")
        {
        }

        public DgvColAttribute(string colName, int index, bool isReadonly, string format)
            : this(colName, index, isReadonly, DEFAULT_AUTO_SIZE_COLUMN_MODE, format) {
        }

        public DgvColAttribute(string colName, int index, bool isReadonly, DataGridViewAutoSizeColumnMode mode, string format) {
            Name = colName;
            Index = index;
            ReadOnly = isReadonly;
            Mode = mode;
            Format = format;
        }
    }
}