using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class MaintenanceInfo : IMaintenanceInfo
    {
        public HashSet<int> GameGuessHistory { get; private set; }

        public MaintenanceInfo()
        {
            GameGuessHistory = new HashSet<int>();
        }

        public void AddGuessHistoryItem(int value)
        {
            GameGuessHistory.Add(value);
        }
    }
}
