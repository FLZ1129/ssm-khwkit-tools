using System.Data;

namespace CrazySharp.Base.Interfaces
{
    /// <summary>
    /// 支持从FromDataRow解析
    /// </summary>
    public interface IFromDataRow
    {
        bool FromDataRow<T>(DataRow row, string prefix = "");
    }
}