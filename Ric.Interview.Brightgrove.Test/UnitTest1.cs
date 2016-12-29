using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Ric.GuessGame.Factories;
using Ric.GuessGame.Models;
using Moq;
using System.Linq;
using Ric.GuessGame.Algorythms;
using Ric.GuessGame.Utils;
using System.Diagnostics;
using Ric.GuessGame.Extentions;
using System.Reflection;
using Ric.GuessGame.Presentation;

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
        const int MaxFruits = 1040;
        const int MaxAttempts = 100;
        const int MaxMilliseconds = 250000;
        const int SecretValue = 910;

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
        public void GameOutputTest()
        {
            var mi = new MaintenanceInfo();
            var plJohn = new Mock<IGuessGamePlayer>();
            plJohn.SetupGet(p => p.Name).Returns("John");
            var plWill = new Mock<IGuessGamePlayer>();
            plWill.SetupGet(p => p.Name).Returns("Will");
            mi.AddGuessHistoryItem(80, plJohn.Object);
            mi.AddGuessHistoryItem(90, plWill.Object);
            var gameOutput = mi.GetGameOutput(91);
            Assert.AreEqual(90, gameOutput.WinnersBestGuess);
            Assert.AreEqual("Will", gameOutput.WinnerPlayer.Name);
            Assert.AreEqual(91, gameOutput.SecretValue);
            Assert.AreEqual(1, gameOutput.NumberOfAttempts);
        }
        [TestMethod]
        public void TestCheatersCreation()
        {
            var mimoq = new Mock<IMaintenanceInfo>();
            var chalg = new CheatPippingGuessHistory(mimoq.Object);
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
        public void TestGameRuler_Semaphore()
        {
            var log = new Mock<ILogger>();
            log.Setup(l => l.AddLogItem(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((fmt, args) => { { Log(fmt, args); } });

            try
            {
                using (var gr = GameHostFactory.GetGameHost(
                    GetGameRules(),
                    GetGameResolver(),
                    PlayerFactoryParserJson.NewJsonPlayer(InputJson),
                    log.Object))
                {
                    gr.StartGame();

                    Log("=============================================");
                    Assert.IsTrue(gr.TotalAttemptsCount <= MaxAttempts);
                    Assert.IsTrue(gr.TotalAttemptsCount > 0);
                    Log("Total attempts count: {0}", gr.TotalAttemptsCount);

                    var go = gr.GameOutput;
                    Log("Winner player {0}", go.WinnerPlayer.Name);
                    Log("Number of attempts {0}", go.NumberOfAttempts);
                    Log("Winner's best guess {0}", go.WinnersBestGuess);
                    Log("Secret value {0}", go.SecretValue);
                }
            }
            catch(OperationCanceledException) { }
            catch (Exception e)
            {
                var ex = e;
                while (ex != null)
                {
                    Log(ex.Message, ex.StackTrace);
                    ex = ex.InnerException;
                }
                Assert.Fail();
            }

        }
    }
}
