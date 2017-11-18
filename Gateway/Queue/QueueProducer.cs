using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.Queue
{
    public class QueueProducer
    {
        private static ConcurrentQueue<Func<Task<bool>>> queue = new ConcurrentQueue<Func<Task<bool>>>();
        private static Timer timer = new Timer(TimerCallback, null, 0, 100);

        public static void GetIntoQueueTillSuccess(Func<Task<bool>> func) => queue.Enqueue(func);

        private static void TimerCallback(object o)
        {
            Func<Task<bool>> func = null;
            while (queue.Count > 0)
                if (queue.TryDequeue(out func) && !func().Result)
                    queue.Enqueue(func);
        }
    }
}
