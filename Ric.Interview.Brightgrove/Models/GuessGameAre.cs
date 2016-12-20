using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameAre : IGuessGame<int>
    {
        private IGameRules rules;
        private IGameResolver resolver;
        private MaintenanceInfo guessedValues;

        public GameOutput Output { get; private set; }
        public ILogger Logger { get; private set; }
        public GuessGameAre(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger)
        {
            this.rules = rules;
            this.resolver = resolver;
            guessedValues = mi as MaintenanceInfo;
            Output = new GameOutput();
            this.Logger = logger;
        }

        public int ValidateGuess(Player playerGuess)
        {
            var guessVal = playerGuess.Guess();
            guessedValues.AddGuessHistoryItem(guessVal);
            Output.Add(playerGuess, guessVal);
            Logger.AddLogItem("Player {0} made a guess {1}", playerGuess.Name, guessVal);

            return Math.Abs(resolver.SecretValue - guessVal);
        }
    }
}
