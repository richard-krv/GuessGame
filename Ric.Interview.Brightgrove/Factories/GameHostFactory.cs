﻿using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public static class GameHostFactory
    {
        public static IGameAIHost GetGameAIHost(string key, IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            var keyl = key.ToLowerInvariant();
            if (keyl == "awaitable")
                return new GuessGameAwaitableFailHost(gameRules, gameResolver, playersIncome, logger);
            else if (keyl == "inlinedelay")
                return new GuessGameInlineDelayHost(gameRules, gameResolver, playersIncome, logger);
            return null;
        }
    }
}
