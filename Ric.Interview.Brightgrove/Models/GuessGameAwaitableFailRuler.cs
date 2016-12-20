using Ric.Interview.Brightgrove.FruitBasket.Factories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.Interview.Brightgrove.FruitBasket.Extentions;
using System.Collections.Concurrent;
using Ric.Interview.Brightgrove.FruitBasket.Utils;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameAwaitableFailRuler
    {
        private ConcurrentQueue<Player> players;
        private CancellationTokenSource ctSrc;
        public IGuessGameEvents<Task> game { get; private set; }
        private readonly ILogger logger;
        public CancellationToken GameState { get { return ctSrc.Token; } }
        public GuessGameAwaitableFailRuler(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            this.logger = logger;

            var mi = new MaintenanceInfo();
            game = GameFactory.GetGame<Task>(gameRules, gameResolver, mi, logger) as IGuessGameEvents<Task>;

            game.GuessFailed += Game_OnFailedGuess;
            game.GuessSucceeded += Game_OnSuccessGuess;

            // init players
            this.players = playersIncome.ToConcurrentQueue(gameRules, gameResolver, mi);

            // devise a game finish condition
            ctSrc = new CancellationTokenSource(gameResolver.MaxMilliseconds);
            //cplSrc = new TaskCompletionSource<object>();
            //ctSrc.Token.Register(() => cplSrc.TrySetCanceled());
            (game as ICancellableGame).SetCancellactionToken(ctSrc.Token);

        }

        private void Game_OnSuccessGuess(Player winPlayer)
        {
            logger.AddLogItem("Player {0} has successfully guessed the secret number", winPlayer.Name);
            ctSrc.Cancel(true);
        }

        private async void Game_OnFailedGuess(Player p, int penalty)
        {
            logger.AddLogItem("Player {0} is waiting {1}", p.Name, penalty);
            try
            {
                ctSrc.Token.ThrowIfCancellationRequested();
                await Task.Delay(penalty, ctSrc.Token);
                logger.AddLogItem("Player {0} returning back to the game. Players {1}", p.Name, players.Count);
                players.Enqueue(p);
            }
            catch (OperationCanceledException ce)
            {
                logger.AddLogItem("Game has been cancelled");
            }
        }

        public void StartGame()
        {
            try
            {
                logger.AddLogItem("Start game: before init game start");
                InitiateGameStart(ctSrc.Token);
                logger.AddLogItem("Start game: after init game start");
            }
            catch (AggregateException aex)
            {
                logger.AddLogItem("Aggregate exception has occured {0}, {1}, count: {2}",
                    aex.Message, aex.StackTrace, aex.InnerExceptions.Count);
                foreach (var e in aex.InnerExceptions)
                    logger.AddLogItem(e.Message, e.StackTrace);
            }
            catch (OperationCanceledException e)
            {
                logger.AddLogItem("Game has been cancelled");
            }
            finally
            {
                logger.AddLogItem("Game finished");
            }
        }

        private Task InitiateGameStart(CancellationToken ctoken)
        {
            var sw = new SpinWait();
            while (true)
            {
                ctoken.ThrowIfCancellationRequested();
                Player player;
                if (players.TryDequeue(out player))
                    game.ValidateGuess(player);
                else
                {
                    logger.AddLogItem("empty queue --------------------------------------");
                    sw.SpinOnce();
                }
            }
        }
    }
}
