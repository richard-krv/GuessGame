using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public static class GameFactory
    {
        public static IGuessGame<T> GetGame<T>(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger)
        {
            if (typeof(T) == typeof(int))
                return new GuessGameAre(rules, resolver, mi, logger) as IGuessGame<T>;
            if (typeof(T) == typeof(Task))
                return new GuessGameAwaitableFail(rules, resolver, mi, logger) as IGuessGame<T>;
            if (typeof(T) == typeof(Task<int>))
                return new GuessGameReturnTimeout(rules, resolver, mi, logger) as IGuessGame<T>;

            return default(IGuessGame<T>);
        }
    }
}
