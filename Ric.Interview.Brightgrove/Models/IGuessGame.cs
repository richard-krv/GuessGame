using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGuessGame<out T>
    {
        T ValidateGuess(Player playerGuess);
        GameLog GameLog { get; }
        IGameOutput GetGameOutput();
    }
}
