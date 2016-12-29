using Ric.GuessGame.Presentation;

namespace Ric.GuessGame.Models
{
    internal class GameOutput: IGameOutput
    {
        public int SecretValue { get; private set; }
        public IGuessGamePlayer WinnerPlayer { get; private set; }
        public int WinnersBestGuess { get; private set; }
        public int NumberOfAttempts { get; private set; }

        public GameOutput(int secretVal, IGuessGamePlayer winnerPlayer, int winnerNumber, int numberOfAttempts)
        {
            SecretValue = secretVal;
            WinnerPlayer = winnerPlayer;
            WinnersBestGuess = winnerNumber;
            NumberOfAttempts = numberOfAttempts;
        }
    }
}
