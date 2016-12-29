using Ric.GuessGame.GameAICore;
using Ric.GuessGame.Models;
using Ric.GuessGame.Presentation;
using Ric.GuessGame.Utils;
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
