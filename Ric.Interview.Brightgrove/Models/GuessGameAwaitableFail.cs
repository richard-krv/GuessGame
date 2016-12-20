using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameAwaitableFail : IGuessGameEvents<Task>, ICancellableGame
    {
        private IGameRules rules;
        private IGameResolver resolver;
        private MaintenanceInfo guessedValues;
        private CancellationToken cToken;

        public GameOutput Output { get; private set; }
        public ILogger Logger { get; private set; }
        public GuessGameAwaitableFail(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger)
        {
            this.rules = rules;
            this.resolver = resolver;
            guessedValues = mi as MaintenanceInfo;
            Output = new GameOutput();
            this.Logger = logger;
        }

        public event Action<Player, int> GuessFailed;
        public event Action<Player> GuessSucceeded;

        public Task ValidateGuess(Player playerGuess)
        {
            cToken.ThrowIfCancellationRequested();

            var guessVal = playerGuess.Guess();
            guessedValues.AddGuessHistoryItem(guessVal);
            Output.Add(playerGuess, guessVal);
            
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
