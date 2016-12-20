using System.Collections.Generic;
using Ric.Interview.Brightgrove.FruitBasket.Factories;
using System.Collections.Concurrent;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GameOutput
    {
        public ConcurrentStack<KeyValuePair<Player, int>> GuessHistory { get; private set; }

        public GameOutput()
        {
            GuessHistory = new ConcurrentStack<KeyValuePair<Player, int>>();
        }

        public void Add(Player player, int guess)
        {
            GuessHistory.Push(new KeyValuePair<Player, int>(player, guess));
        }
        
    }
}
