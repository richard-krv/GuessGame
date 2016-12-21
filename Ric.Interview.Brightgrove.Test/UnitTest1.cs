using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Moq;
using System.Linq;
using Ric.Interview.Brightgrove.FruitBasket.Algorythms;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Diagnostics;
using Ric.Interview.Brightgrove.FruitBasket.Extentions;
using System.Reflection;
using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.GuessGame.Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string InputJson = @"{ ""players"" : [
                { ""Name"" : ""John"", ""Type"" : ""Random"" },
                { ""Name"" : ""Paul"", ""Type"" : ""Memory"" },
                { ""Name"" : ""Webb"", ""Type"" : ""Thorough"" },
                { ""Name"" : ""Sack"", ""Type"" : ""Cheater"" },
                { ""Name"" : ""Rick"", ""Type"" : ""ThoroughCheater"" }
            ]}";

        [TestMethod]
        public void TestJsonParser()
        {
            var pp = PlayerFactoryParserJson.NewJsonPlayer(InputJson);
            Assert.IsNotNull(pp);
            Assert.AreEqual(5, new List<IParserPlayer>(pp).Count);
            foreach(var p in pp)
                switch (p.Name)
                {
                    case "John": Assert.AreEqual("Random", p.Type); break;
                    case "Paul": Assert.AreEqual("Memory", p.Type); break;
                    case "Webb": Assert.AreEqual("Thorough", p.Type); break;
                    case "Sack": Assert.AreEqual("Cheater", p.Type); break;
                    case "Rick": Assert.AreEqual("ThoroughCheater", p.Type); break;
                    default: Assert.Fail("Unknown player name was parsed"); break;
                }
        }

        const int MinFruits = 40;
        const int MaxFruits = 145;
        const int MaxAttempts = 100;
        const int MaxMilliseconds = 200000;
        const int SecretValue = 91;

        public IGameResolver GetGameResolver()
        {
            var gr = new Mock<IGameResolver>();
            gr.SetupGet(r => r.MaxAttempts).Returns(MaxAttempts);
            gr.SetupGet(r => r.MaxMilliseconds).Returns(MaxMilliseconds);
            gr.SetupGet(r => r.SecretValue).Returns(SecretValue);

            return gr.Object;
        }
        public IGameRules GetGameRules()
        {
            var gamerules = new Mock<IGameRules>();
            gamerules.SetupGet(r => r.MaxValue).Returns(MaxFruits);
            gamerules.SetupGet(r => r.MinValue).Returns(MinFruits);

            return gamerules.Object;
        }
        public IEnumerable<Player> GetPlayers()
        {
            var players = PlayerFactoryParserJson.NewJsonPlayer(InputJson);
            var gamerules = GetGameRules();

            var result = players.Select(p => Player.NewPlayer(p, gamerules));
            return result;
        }
        [TestMethod]
        public void TestFactoryParser()
        {
            var players = GetPlayers().ToList();
            Assert.AreEqual(5, players.Count);
            foreach (var p in players)
                switch (p.Type)
                {
                    case PlayerType.Random: Assert.AreEqual("John", p.Name); break;
                    case PlayerType.Memory: Assert.AreEqual("Paul", p.Name); break;
                    case PlayerType.Thorough: Assert.AreEqual("Webb", p.Name); break;
                    case PlayerType.Cheater: Assert.AreEqual("Sack", p.Name); break;
                    case PlayerType.ThoroughCheater: Assert.AreEqual("Rick", p.Name); break;
                    default: Assert.Fail("Unknown player name was parsed"); break;
                }
        }
        private void Log(string format, params object[] args)
        {
            var t = string.Format("{0} ", DateTime.Now.ToString("HH:mm:ss.fff"));
            Debug.WriteLine(t + format, args);
        }
        [TestMethod]
        public void TestCheatersCreation()
        {
            var guessHistory = new HashSet<int>();
            guessHistory.Add(40);

            var mi = new Mock<IMaintenanceInfo>();
            mi.SetupGet(m => m.GameGuessHistory).Returns(guessHistory);
            var chalg = new CheatPippingGuessHistory(mi.Object);

            var players = GetPlayers();
            var chplayers = players.InitCheaters(chalg);
            Assert.AreEqual(5, chplayers.Count());
            var cheaters = chplayers.Where(p => p is ICheaterPlayer);
            Assert.AreEqual(2, cheaters.Count());

            foreach (var ch in cheaters)
            {
                var cheatAlg = ch.GetType().GetField("cheatAlg",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.GetField).GetValue(ch);
                Assert.AreEqual(chalg, cheatAlg);
            }
        }
        [TestMethod]
        public void TestGameRuler_TaskDelay()
        {
            var log = new Mock<ILogger>();
            log.Setup(l => l.AddLogItem(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((fmt, args) => { { Log(fmt, args); } });

            try
            {
                using (var gr = new GuessGameInlineDelayHost(
                    GetGameRules(),
                    GetGameResolver(),
                    PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                    log.Object))
                {
                    gr.StartGame();

                    Debug.WriteLine("Is cancellation requested: {0}", gr.IsCancellationRequested);

                    Assert.IsTrue(gr.GameLog.GuessHistory.Count <= MaxAttempts);
                    Assert.IsTrue(gr.GameLog.GuessHistory.Count > 0);

                    foreach (var p in gr.GameLog.GuessHistory)
                        Debug.WriteLine("Output: {0}, guess {1}", p.Key.Name, p.Value);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message, e.StackTrace);
                var ex = e.InnerException;
                while (ex != null)
                {
                    Debug.WriteLine(ex.Message, ex.StackTrace);
                    ex = ex.InnerException;
                }
            }
        }
        [TestMethod]
        public void TestGameRuler_AwaitEvent()
        {
            var log = new Mock<ILogger>();
            log.Setup(l => l.AddLogItem(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((fmt, args) => { { Log(fmt, args); } });

            try
            {
                using (var gr = new GuessGameAwaitableFailHost(
                    GetGameRules(),
                    GetGameResolver(),
                    PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                    log.Object))
                {
                    gr.StartGame();

                    Debug.WriteLine("Is cancellation requested: {0}", gr.IsCancellationRequested);

                    Assert.IsTrue(gr.GameLog.GuessHistory.Count <= MaxAttempts);
                    Assert.IsTrue(gr.GameLog.GuessHistory.Count > 0);

                    foreach (var p in gr.GameLog.GuessHistory)
                        Debug.WriteLine("Output: {0}, guess {1}", p.Key.Name, p.Value);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message, e.StackTrace);
                var ex = e.InnerException;
                while (ex != null)
                {
                    Debug.WriteLine(ex.Message, ex.StackTrace);
                    ex = ex.InnerException;
                }
            }

        }
    }
}
