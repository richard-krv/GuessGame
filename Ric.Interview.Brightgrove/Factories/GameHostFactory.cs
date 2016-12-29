using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Generic;

namespace Ric.GuessGame.Factories
{
    public static class GameHostFactory
    {
        public static IGameAIHost GetGameHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            return SemaphoreHost.GetGameHost(gameRules, gameResolver, playersIncome, logger);
        }
    }
}
