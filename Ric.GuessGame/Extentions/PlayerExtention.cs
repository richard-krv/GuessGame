using Ric.GuessGame.Algorythms;
using Ric.GuessGame.Factories;
using Ric.GuessGame.Models;
using Ric.GuessGame.Presentation;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ric.GuessGame.Extentions
{
    public static class PlayerExtention
    {
        public static ConcurrentQueue<IGuessGamePlayer> ToConcurrentQueue(
            this IEnumerable<IParserPlayer> playersIncome, IGameRules gameRules,
            IGameResolver gameResolver, IMaintenanceInfo mi)
        {
            var players = new List<Player>(playersIncome.Count());

            players.AddRange(playersIncome.Select(p => 
                Player.NewPlayer(p, gameRules)));

            var chalg = new CheatPippingGuessHistory(mi);
            var chPlayers = players.InitCheaters(chalg);

            var thisplayers = new ConcurrentQueue<IGuessGamePlayer>();
            foreach (var p in chPlayers)
                thisplayers.Enqueue(p);

            return thisplayers;
        }

        public static IEnumerable<IGuessGamePlayer> InitCheaters(this IEnumerable<IGuessGamePlayer> players,
            ICheatingAlgorithm chalg)
        {
            return players.Select(p => CheaterPlayerFactory
                .InitializeCheaterPlayer(p as ICheaterPlayer, chalg) as Player ?? p);
        }
    }
}
