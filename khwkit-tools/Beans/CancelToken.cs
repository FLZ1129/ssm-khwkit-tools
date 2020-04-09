using System.Threading;
using System.Threading.Tasks;

namespace khwkit.Beans
{
    public class CancelToken
    {
        private long flag = 1;

        public void Cancel()
        {
            Interlocked.Exchange(ref flag, 0);
        }

        public void CancelAfter(uint duration)
        {
            Task.Run(() => {
                Thread.Sleep((int)duration);
                Interlocked.Exchange(ref flag, 0);
            });
        }

        public bool IsCanceled()
        {
            return Interlocked.Read(ref flag) == 0;
        }
    }
}
