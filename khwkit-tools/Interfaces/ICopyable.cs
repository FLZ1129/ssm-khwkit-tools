using CrazySharp.Std;
using System;

namespace CrazySharp.Base.Interfaces
{
    /// <summary>
    /// 支持深拷贝
    /// </summary>
    public interface ICopyable
    {
    }

    /// <summary>
    /// 支持在DataGridView中显示的数据模型
    /// </summary>

    [Serializable]
    public abstract class CopableImpl<T> : ICopyable where T : class, ICopyable
    {
        protected virtual void AfterCopy(T newCopy) {
        }

        public T Copy() {
            var newCopy = Extensions.Copy(this as T);
            AfterCopy(newCopy);
            return newCopy;
        }
    }
}