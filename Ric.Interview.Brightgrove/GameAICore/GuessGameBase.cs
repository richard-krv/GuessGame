using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    internal abstract class GuessGameBase<T> : IGuessGame<T>
    {
        public abstract T ValidateGuess(Player playerGuess);

        public GameLog GameLog { get; private set; }
        public ILogger Logger { get; private set; }

        protected IGameRules rules;
        protected IGameResolver resolver;
        protected MaintenanceInfo guessedValues;

        public GuessGameBase(IGameRules rules, IGameResolver resolver,
            IMaintenanceInfo mi, ILogger logger)
        {
            this.rules = rules;
            this.resolver = resolver;
            guessedValues = mi as MaintenanceInfo;
            GameLog = new GameLog();
            this.Logger = logger;
        }

        public IGameOutput GetGameOutput()
        {
            return new GameOutput(resolver, guessedValues);
        }
    }
}
