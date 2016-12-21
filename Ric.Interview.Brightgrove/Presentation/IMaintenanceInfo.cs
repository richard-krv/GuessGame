using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IMaintenanceInfo
    {
        HashSet<int> GameGuessHistory { get; }

        bool Contains(int guess);
    }
}
