using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGuessGame<out T>
    {
        T ValidateGuess(Player playerGuess);
        GameOutput Output { get; }
    }
}
