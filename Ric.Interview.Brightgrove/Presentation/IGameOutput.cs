using Ric.Interview.Brightgrove.FruitBasket.Models;

namespace Ric.Interview.Brightgrove.FruitBasket.Presentation
{
    public interface IGameOutput
    {
        int SecretValue { get; }
        Player WinnerPlayer { get; }
        int NumberOfAttempts { get; }

    }
}
