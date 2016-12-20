using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Algorythms
{
    public interface ICheatingAlgorithm
    {
        int Guess(Func<int> straightGuess);
    }
}
