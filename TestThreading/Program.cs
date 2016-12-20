using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                test4();
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Console.WriteLine("Game over");
            }

            Console.ReadKey();
        }
        static ConcurrentQueue<Tuple<int, string>> players;
        const int GameTimeout = 10000;
        static void test4()
        {
            players = new ConcurrentQueue<Tuple<int, string>>()
            {
                new Tuple<int, string>(4, "thread 1"),
                new Tuple<int, string>(6, "thread 2"),
                new Tuple<int, string>(4, "thread 3"),
                new Tuple<int, string>(2, "thread 4"),
                new Tuple<int, string>(1, "thread 5"),
            };

            var cts = new CancellationTokenSource(GameTimeout);

            var sw = new SpinWait();
            var waitwrite = true;
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                Tuple<int, string> p;
                if (players.TryDequeue(out p))
                {
                    Console.WriteLine("Players count {0}", players.Count);
                    waitwrite = true;
                    DoSynchWork(800, p.Item2);
                    Task.Factory.StartNew(() => GetDelay(p, cts.Token), cts.Token);
                }
                else
                {
                    if (waitwrite)
                    {
                        Console.WriteLine("Queue is empty - spinning--------------");
                        waitwrite = false;
                    }
                    sw.SpinOnce();
                }
            }
        }
        static Task GetDelay(Tuple<int, string> p, CancellationToken ct)
        {
            Console.WriteLine("Task {0} delay starting", p.Item2);
            return Task.Delay(TimeSpan.FromSeconds(p.Item1), ct).ContinueWith(t =>
            {
                players.Enqueue(p);
                Console.WriteLine("Task {0} delay completed - player back to play", p.Item2);
            });
        }
        static void test3()
        {
            var e = new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(4, "thread 1"),
                new Tuple<int, string>(6, "thread 2"),
                new Tuple<int, string>(4, "thread 3"),
                new Tuple<int, string>(2, "thread 4"),
                new Tuple<int, string>(1, "thread 5"),
            };
            foreach (var el in e)
            {
                DoSynchWork(1000, el.Item2);
                Task.Factory.StartNew(() => GetDelay(el.Item1, el.Item2));
                //Thread.Sleep(1100);
            }
        }

        static void DoSynchWork(int workMs, string taskName)
        {
            Console.WriteLine("Task {0} starting", taskName);
            //Thread.Sleep(workMs);
            var r = new Random(workMs);
            var l = new SortedList<string, string>();
            for (int i = 0; i < workMs; i++)
                l.Add(i.ToString(), (r.Next(2, workMs) * 2 / 3).ToString());
            Console.WriteLine("Task {0} ended", taskName);
        }

        static void test2()
        {
            var e = new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(4, "thread 1"),
                new Tuple<int, string>(6, "thread 2"),
                new Tuple<int, string>(4, "thread 3"),
                new Tuple<int, string>(2, "thread 4"),
                new Tuple<int, string>(1, "thread 5"),
            };
            foreach(var el in e)
            {
                Task.Factory.StartNew(() => GetDelay(el.Item1, el.Item2));
                Thread.Sleep(1100);
            }
        }

        static Task GetDelay(int delaySeconds, string name)
        {
            Console.WriteLine("Task {0} delay starting", name);
            return Task.Delay(TimeSpan.FromSeconds(delaySeconds)).ContinueWith(t => 
                Console.WriteLine("Task {0} delay completed", name));
        }

        static void test1()
        {
            Task wait = asyncTask();
            syncCode();
            //wait.Wait();
            Console.ReadLine();
        }

        static async Task asyncTask()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("async: Starting");
            Task wait = Task.Delay(5000);
            Console.WriteLine("async: Running for {0} seconds", DateTime.Now.Subtract(start).TotalSeconds);
            await wait;
            Console.WriteLine("async: Running for {0} seconds", DateTime.Now.Subtract(start).TotalSeconds);
            Console.WriteLine("async: Done");
        }

        static void syncCode()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("sync: Starting");
            Thread.Sleep(5000);
            Console.WriteLine("sync: Running for {0} seconds", DateTime.Now.Subtract(start).TotalSeconds);
            Console.WriteLine("sync: Done");
        }

    }
}
