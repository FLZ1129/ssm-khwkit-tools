using System;
using System.Threading;

namespace CrazySharp.Std
{
    /// <summary>
    /// 线性等待 Once
    /// </summary>
    public class Once : IDisposable
    {
        private long flag;
        private Mutex mtx;

        public Once() {
            flag = 0;
            mtx = new Mutex();
        }

        ~Once() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool fromUser) {
            if (fromUser)
            {
                mtx.Dispose();
            }
        }

        public void Do(Action a) {
            if (Interlocked.Read(ref flag) == 1)
            {
                return;
            }
            Utils.MutexOperation(mtx, () => {
                a.Invoke();
                Interlocked.Exchange(ref flag, 1);
            });
        }
    }
}