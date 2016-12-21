using System.Collections.Generic;
using Ric.Interview.Brightgrove.FruitBasket.Factories;
using System.Collections.Concurrent;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GameLog
    {
        public ConcurrentStack<KeyValuePair<Player, int>> GuessHistory { get; private set; }

        public GameLog()
        {
            GuessHistory = new ConcurrentStack<KeyValuePair<Player, int>>();
        }

        public void Add(Player player, int guess)
        {
            GuessHistory.Push(new KeyValuePair<Player, int>(player, guess));
        }
        
    }
}
