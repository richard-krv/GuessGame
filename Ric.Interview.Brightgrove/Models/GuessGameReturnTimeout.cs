using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameReturnTimeout : IGuessGameEvents<Task<int>>, ICancellableGame
    {
        private IGameRules rules;
        private IGameResolver resolver;
        private MaintenanceInfo guessedValues;
        private CancellationToken cToken;

        public GameOutput Output { get; private set; }
        public ILogger Logger { get; private set; }
        public GuessGameReturnTimeout(IGameRules rules, IGameResolver resolver,
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

        public async Task<int> ValidateGuess(Player playerGuess)
        {
            cToken.ThrowIfCancellationRequested();

            var guessVal = playerGuess.Guess();
            guessedValues.AddGuessHistoryItem(guessVal);
            Output.Add(playerGuess, guessVal);

            Logger.AddLogItem("Player {0} made a guess {1}", playerGuess.Name, guessVal);

            var wait = await Task.Run(() => Math.Abs(resolver.SecretValue - guessVal) * 100);
            Logger.AddLogItem("Player {0} should wait for {1} seconds", playerGuess.Name, wait / 1000.0);
            return wait;
        }

        public void SetCancellactionToken(CancellationToken token)
        {
            this.cToken = token;
        }
    }
}
