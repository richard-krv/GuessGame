using Ric.GuessGame.Models;

namespace Ric.GuessGame.Presentation
{
    public interface IGameOutput
    {
        int SecretValue { get; }
        IGuessGamePlayer WinnerPlayer { get; }
        int WinnersBestGuess { get; }
        int NumberOfAttempts { get; }

    }
}
