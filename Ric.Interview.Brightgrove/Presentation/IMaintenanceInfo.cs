using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Presentation
{
    public interface IMaintenanceInfo
    {
        HashSet<int> GameGuessHistory { get; }

        bool Contains(int guess);
    }
}
