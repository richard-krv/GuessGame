using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    internal class GuessGameReturnDelay : GuessGameBase<int>
    {
        public GuessGameReturnDelay(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger) : base(rules, resolver, mi, logger)
        {
        }

        public override int ValidateGuess(Player playerGuess)
        {
            var guessVal = playerGuess.Guess();
            guessedValues.AddGuessHistoryItem(guessVal, playerGuess);
            GameLog.Add(playerGuess, guessVal);
            Logger.AddLogItem("Player {0} made a guess {1}", playerGuess.Name, guessVal);

            return Math.Abs(resolver.SecretValue - guessVal);
        }

    }
}
