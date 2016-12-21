using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Threading;

namespace Ric.Interview.Brightgrove.FruitBasket.PresenterConsole
{
    class Program
    {
        private const string InputJson = @"{ ""players"" : [
                { ""Name"" : ""John"", ""Type"" : ""Random"" },
                { ""Name"" : ""Paul"", ""Type"" : ""Memory"" },
                { ""Name"" : ""Webb"", ""Type"" : ""Thorough"" },
                { ""Name"" : ""Sack"", ""Type"" : ""Cheater"" },
                { ""Name"" : ""Rick"", ""Type"" : ""ThoroughCheater"" }
            ]}";

        static CancellationTokenSource ts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        static ILogger logger;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start game (AI mode)");
            Console.ReadKey();

            logger = new Logger();
            try
            {
                using (var h = GetAwaitableFailHost()
                               //GetInlineDelayHost()
                    )
                    h.StartGame();
            }
            catch (Exception ex)
            {
                logger.AddLogItem("{0} {1}", ex.Message, ex.StackTrace);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        public static IGameAIHost GetInlineDelayHost()
        {
            return new GuessGameInlineDelayHost(
                    GetGameRules(),
                    GetGameResolver(),
                    PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                    logger
                    );
        }

        public static IGameAIHost GetAwaitableFailHost()
        {
            return new GuessGameAwaitableFailHost(
                    GetGameRules(),
                    GetGameResolver(),
                    PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                    logger
                    );
        }

        private static IGameResolver GetGameResolver()
        {
            return new Resolver();
        }

        private static IGameRules GetGameRules()
        {
            return new Rules();
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

        class Resolver : IGameResolver
        {
            public int MaxAttempts { get { return 100; } }
            public int MaxMilliseconds { get { return 200000; } }
            public int SecretValue { get { return 91; } }
        }

        class Rules : IGameRules
        {
            public int MaxValue { get { return 140; } }

            public int MinValue { get { return 40; } }
        }
    }
}
