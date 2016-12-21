using Ric.Interview.Brightgrove.FruitBasket.Factories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    public class GuessGameAwaitableFailHost: GuessGameHostBase
    {
        internal IGuessGameEvents<Task> game { get; private set; }

        public override GameLog GameLog { get { return game.GameLog; } }

        public GuessGameAwaitableFailHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
                : base(gameRules, gameResolver, playersIncome, logger)
        {
            game = GameFactory.GetGame<Task>(gameRules, gameResolver, mi, logger) as IGuessGameEvents<Task>;

            game.GuessFailed += Game_OnFailedGuess;
            game.GuessSucceeded += Game_OnSuccessGuess;

            (game as ICancellableGame).SetCancellactionToken(ctSrc.Token);
        }

        private void Game_OnSuccessGuess(Player winPlayer)
        {
            logger.AddLogItem("Player {0} has successfully guessed the secret number", winPlayer.Name);
            ctSrc.Cancel(true);
        }

        private async void Game_OnFailedGuess(Player p, int penalty)
        {
            try
            {
                ctSrc.Token.ThrowIfCancellationRequested();
                logger.AddLogItem("Player {0} is waiting {1}", p.Name, penalty);

                await Task.Delay(penalty, ctSrc.Token);

                logger.AddLogItem("Player {0} returning back to the game. Players {1}", p.Name, players.Count);
                players.Enqueue(p);
            }
            catch (OperationCanceledException) { }
        }

        protected override void InitiateGameStart(CancellationToken ctoken)
        {
            var sw = new SpinWait();
            var spinlog = true;
            while (!ctoken.IsCancellationRequested)
            {
                spinlog = true;
                ctoken.ThrowIfCancellationRequested();
                Player player;
                if (players.TryDequeue(out player))
                    game.ValidateGuess(player);
                else
                {
                    if (spinlog && players.Count == 0)
                    {
                        logger.AddLogItem("empty queue --------------------------------------");
                        spinlog = false;
                    }
                    sw.SpinOnce();
                }
            }
        }
    }
}
