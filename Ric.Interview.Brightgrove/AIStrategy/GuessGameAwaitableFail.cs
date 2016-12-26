using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Threading.Tasks;
using System.Threading;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    internal class GuessGameAwaitableFail : GuessGameBase<Task>, IGuessGameEvents<Task>, ICancellableGame
    {
        private CancellationToken cToken;

        public GuessGameAwaitableFail(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger) : base(rules, resolver, mi, logger)
        {
        }

        public event Action<Player, int> GuessFailed;
        public event Action<Player> GuessSucceeded;

        public override Task ValidateGuess(Player playerGuess)
        {
            cToken.ThrowIfCancellationRequested();

            var guessVal = playerGuess.Guess();
            guessedValues.AddGuessHistoryItem(guessVal, playerGuess);
            GameLog.Add(playerGuess, guessVal);
            
            Logger.AddLogItem("Player {0} made a guess {1}", playerGuess.Name, guessVal);

            if (guessVal == resolver.SecretValue)
                GuessSucceeded(playerGuess);
            else
                GuessFailed(playerGuess, Math.Abs(resolver.SecretValue - guessVal));

            return null;
        }
        public void SetCancellactionToken(CancellationToken token)
        {
            this.cToken = token;
        }
    }
}
