using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Utils
{
    public interface ILogger
    {
        void AddLogItem(string format, params object[] args);
    }
}
