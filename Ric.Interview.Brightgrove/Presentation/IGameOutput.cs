using Ric.Interview.Brightgrove.FruitBasket.Models;

namespace Ric.Interview.Brightgrove.FruitBasket.Presentation
{
    public interface IGameOutput
    {
        int SecretValue { get; }
        IGuessGamePlayer WinnerPlayer { get; }
        int WinnersBestGuess { get; }
        int NumberOfAttempts { get; }

    }
}
