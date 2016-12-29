using Ric.GuessGame.Models;

namespace Ric.GuessGame.Presentation
{
    public interface IMaintenanceInfo
    {
        bool Contains(int guess);
        void AddGuessHistoryItem(int value, IGuessGamePlayer player);
        IGameOutput GetGameOutput(int secretValue);
        int TotalAttemptsCount { get; }
    }
}
