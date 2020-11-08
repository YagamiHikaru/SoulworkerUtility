using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xor
{
    /// <summary>
    ///  封裝ManualResetEvent
    /// </summary>
    public class MutipleThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent done;
        private readonly int total;
        private long current;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="total">需要等待執行的執行緒總數</param>
        public MutipleThreadResetEvent(int total)
        {
            this.total = total;
            current = total;
            done = new ManualResetEvent(false);
        }

        /// <summary>
        /// 喚醒一個等待的執行緒
        /// </summary>
        public void SetOne()
        {
            // Interlocked 原子操作類 ,此處將計數器減1
            if (Interlocked.Decrement(ref current) == 0)
            {
                //當所有等待執行緒執行完畢時，喚醒等待的執行緒
                done.Set();
            }
        }

        /// <summary>
        /// 等待所有執行緒執行完畢
        /// </summary>
        public void WaitAll()
        {
            done.WaitOne();
        }

        /// <summary>
        /// 釋放物件佔用的空間
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)done).Dispose();
        }
    }

    /// <summary>
    ///  封裝ManualResetEvent
    /// </summary>
    public class DynamicMutipleThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent done;
        private long current;

        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="total">需要等待執行的執行緒總數</param>
        public DynamicMutipleThreadResetEvent()
        {
            done = new ManualResetEvent(false);
        }

        /// <summary>
        /// 新增一個執行緒
        /// </summary>
        public void AddOne()
        {
            Interlocked.Increment(ref current);
        }

        /// <summary>
        /// 喚醒一個等待的執行緒
        /// </summary>
        public void SetOne()
        {
            // Interlocked 原子操作類 ,此處將計數器減1
            if (Interlocked.Decrement(ref current) == 0)
            {
                //當所有等待執行緒執行完畢時，喚醒等待的執行緒
                done.Set();
            }
        }

        /// <summary>
        /// 等待所有執行緒執行完畢
        /// </summary>
        public void WaitAll()
        {
            done.WaitOne();
        }

        /// <summary>
        /// 釋放物件佔用的空間
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)done).Dispose();
        }
    }
}
