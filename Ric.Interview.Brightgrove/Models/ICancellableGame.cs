using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface ICancellableGame
    {
        void SetCancellactionToken(CancellationToken token);
    }
}
