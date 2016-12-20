using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.PresenterConsole
{
    class Program
    {
        private static string InputJson;

        static CancellationTokenSource ts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        static void Main(string[] args)
        {
            try
            {
                ts.Token.ThrowIfCancellationRequested();

                for (int i = 0; true; i++)
                {
                    OutputWriteLine("I={0} ", i);
                    Wait(3000, i);
                    Task.Factory.StartNew(() => Task.Delay(1000));
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                Console.ReadKey();
            }
        }

        static async void Wait(int timeout, int threadId)
        {
            ts.Token.ThrowIfCancellationRequested();

            await Task.Factory.StartNew(() => Task.Delay(timeout), ts.Token)
                .ContinueWith(t => OutputWriteLine("thread finished: {0}", threadId));
        }

        static void Main2(string[] args)
        {
            var log = new Logger();

            var gr = new GuessGameRulerAre(
                GetGameRules(),
                GetGameResolver(),
                PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                log);

            try
            {
                gr.StartGame();
            }
            catch (Exception e)
            {
                OutputWriteLine(e.Message, e.StackTrace);
                var ex = e.InnerException;
                while (ex != null)
                {
                    OutputWriteLine(ex.Message, ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

            OutputWriteLine("Is cancellation requested: {0}", gr.GameState.IsCancellationRequested);

            foreach (var p in gr.game.Output.GuessHistory)
                OutputWriteLine("Output: {0}, guess {1}", p.Key.Name, p.Value);
        }

        private static IGameResolver GetGameResolver()
        {
            throw new NotImplementedException();
        }

        private static IGameRules GetGameRules()
        {
            throw new NotImplementedException();
        }

        private static void OutputWriteLine(string format, params object [] args)
        {
            Console.WriteLine(format, args);
        }

        class Logger : ILogger
        {
            private object lockobj = new object();
            public void AddLogItem(string format, params object[] args)
            {
                lock (lockobj)
                    OutputWriteLine(format, args);
            }
        }
    }
}
