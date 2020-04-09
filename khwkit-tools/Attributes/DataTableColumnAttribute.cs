using System;
using System.Diagnostics;
using System.Reflection;

namespace CrazySharp.Base.Attributes
{
    /// <summary>
    /// DataRow
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataTableColumnAttribute : Attribute
    {

        public string Name { get; private set; }
        public int ColumnIndex { get; private set; }

        public string NameWithPrefix(string prefix = "") {
            return $"{prefix}{Name}";
        }
        public DataTableColumnAttribute(string name, int columnIndex = -1)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Name = name;
            ColumnIndex = columnIndex;
        }
    }

    public class PropertyInfoHolder<TAttribute>
    {
        public PropertyInfo Prop { get; set; }
        public TAttribute AttributeData { get; set; }
        public string PropertyName => Prop?.Name ?? "";

        public object GetValue(object obj) {
            return Prop.GetValue(obj);
        }

        public void SetValue(object obj, object value) {
            Prop.SetValue(obj, value);
        }
    }
}