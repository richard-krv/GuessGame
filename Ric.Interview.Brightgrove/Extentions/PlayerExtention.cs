﻿using Ric.Interview.Brightgrove.FruitBasket.Algorythms;
using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ric.Interview.Brightgrove.FruitBasket.Extentions
{
    public static class PlayerExtention
    {
        public static ConcurrentQueue<Player> ToConcurrentQueue(
            this IEnumerable<IParserPlayer> playersIncome, IGameRules gameRules,
            IGameResolver gameResolver, IMaintenanceInfo mi)
        {
            var players = new List<Player>(playersIncome.Count());

            players.AddRange(playersIncome.Select(p => 
                Player.NewPlayer(p, gameRules)));

            var chalg = new CheatPippingGuessHistory(mi);
            var chPlayers = players.InitCheaters(chalg);

            var thisplayers = new ConcurrentQueue<Player>();
            foreach (var p in chPlayers)
                thisplayers.Enqueue(p);

            return thisplayers;
        }

        public static IEnumerable<Player> InitCheaters(this IEnumerable<Player> players,
            ICheatingAlgorithm chalg)
        {
            return players.Select(p => CheaterPlayerFactory
                .InitializeCheaterPlayer(p as ICheaterPlayer, chalg) as Player ?? p);
        }
    }
}
