using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class PlayerActivityLog : IPlayerActivityLog
    {
        private HashSet<int> attempts;
        public PlayerActivityLog() { attempts = new HashSet<int>(); }
        public void LogAttempt(int value) { attempts.Add(value); }
        public int[] Attempts { get { return attempts.ToArray(); } }
    }
}
