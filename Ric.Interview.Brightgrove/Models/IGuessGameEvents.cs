using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGuessGameEvents<T>:IGuessGame<T>
    {
        event Action<Player, int> GuessFailed;
        event Action<Player> GuessSucceeded;
    }
}
