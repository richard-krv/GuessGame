using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public interface IGameResolver
    {
        int SecretValue { get; }
        int MaxAttempts { get; }
        int MaxMilliseconds { get; }
    }
}
