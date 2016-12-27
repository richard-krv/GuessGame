using Ric.Interview.Brightgrove.FruitBasket.Models;
using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Presentation
{
    public interface IMaintenanceInfo
    {
        HashSet<int> GameGuessHistory { get; }
        bool Contains(int guess);
        void AddGuessHistoryItem(int value, IGuessGamePlayer player);
        IGameOutput GetGameOutput(int secretValue);
    }
}
