using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.GameAICore;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Strategy
{
    internal abstract class ObservableGameHost : GuessGameHostBase
    {
        public ObservableGameHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
                : base(gameRules, gameResolver, playersIncome, logger)
        {
            game = GameFactory.GetGame<Task>(gameRules, gameResolver, mi, logger) as IGuessGameEvents<Task>;

            game.GuessFailed += Game_OnFailedGuess;
            game.GuessSucceeded += Game_OnSuccessGuess;

            (game as ICancellableGame).SetCancellactionToken(ctSrc.Token);
        }

        protected abstract void Game_OnSuccessGuess(Player winPlayer);

        protected abstract void Game_OnFailedGuess(Player player, int penalty);

        internal IGuessGameEvents<Task> game { get; private set; }

        public override GameLog GameLog { get { return game.GameLog; } }

    }
}
