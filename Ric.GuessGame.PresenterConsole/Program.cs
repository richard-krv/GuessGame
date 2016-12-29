using Ric.GuessGame.Factories;
using Ric.GuessGame.Models;
using Ric.GuessGame.Presentation;
using Ric.GuessGame.Utils;
using System;

namespace Ric.GuessGame.PresenterConsole
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

        static ILogger logger;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start game (AI mode)");
            Console.ReadKey();

            logger = new Logger();
            try
            {
                using (var h = GetGameHost())
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

        public static IGameAIHost GetGameHost()
        {
            return GameHostFactory.GetGameHost(
                GetGameRules(),
                GetGameResolver(),
                PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                logger);
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
