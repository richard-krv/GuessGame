using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public static class GameHostFactory
    {
        public static IGameAIHost GetGameAIHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            if (true)
                return new GuessGameAwaitableFailHost(gameRules, gameResolver, playersIncome, logger);
            else 
                return new GuessGameInlineDelayHost(gameRules, gameResolver, playersIncome, logger);
        }
    }
}
