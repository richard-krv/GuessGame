using System.Collections.Concurrent;

namespace TestThreading
{
    public static class Extentions
    {
        public static void Add<T>(this ConcurrentQueue<T> cq, T item)
        {
            cq.Enqueue(item);
        }
    }
}
