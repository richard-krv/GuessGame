using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.Interview.Brightgrove.FruitBasket.Extentions;
using System.Collections.Concurrent;
using Ric.Interview.Brightgrove.FruitBasket.Utils;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameRulerAre
    {
        private ConcurrentQueue<Player> players;
        private CancellationTokenSource ctSrc;
        public IGuessGame<int> game { get; private set; }
        private readonly ILogger logger;

        public CancellationToken GameState { get { return ctSrc.Token; } }

        public GuessGameRulerAre(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            this.logger = logger;

            var mi = new MaintenanceInfo();
            game = GameFactory.GetGame<int>(gameRules, gameResolver, mi, logger) as IGuessGame<int>;

            // init players
            this.players = playersIncome.ToConcurrentQueue(gameRules, gameResolver, mi);

            // devise a game finish condition
            ctSrc = new CancellationTokenSource(gameResolver.MaxMilliseconds);
            //cplSrc = new TaskCompletionSource<object>();
            //ctSrc.Token.Register(() => cplSrc.TrySetCanceled());
            //(game as ICancellableGame).SetCancellactionToken(ctSrc.Token);

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
                    //if(e is OperationCanceledException)
                        //logger.AddLogItem("cancelled {0}", (e as OperationCanceledException).CancellationToken.
                    logger.AddLogItem(e.Message, e.StackTrace);
            }
            catch (OperationCanceledException e)
            {
                logger.AddLogItem("Game has been cancelled {0}, {1}", e.Message, e.StackTrace);
            }
            finally
            {
                logger.AddLogItem("Game finished");
            }
        }

        private void InitiateGameStart(CancellationToken ctoken)
        {
            var sw = new SpinWait();
            while (true)
            {
                ctoken.ThrowIfCancellationRequested();
                Player player;
                if (players.TryDequeue(out player))
                {
                    logger.AddLogItem(">> {1}, players in the queue: {0}", players.Count, player.Name);
                    var timeout = game.ValidateGuess(player) * 1000;
                    if (timeout == 0)
                    {
                        logger.AddLogItem("Player {0} has successfully guessed the secret number", player.Name);
                        ctSrc.Cancel(true);
                    }
                    else
                    {
                        logger.AddLogItem("Player {0} is waiting for {1} seconds", player.Name, timeout / 1000);
                        var w = Task.Factory.StartNew(() => Task.Delay(timeout));
                        Task.WhenAll(w).ContinueWith(t => EnqueuePlayer(player));
                        //ImposePenalty(timeout, ctoken, player);
                        /*ApplyPenalty(timeout, ctoken).ContinueWith(t =>
                        {
                            players.Enqueue(player);
                        });*/
                    }
                }
                else
                {
                    logger.AddLogItem("empty queue --------------------------------------");
                    sw.SpinOnce();
                }
            }
        }

        private async void ImposePenalty(int ms, CancellationToken ct, Player player)
        {
            await Task.Factory.StartNew(() => Task.Delay(ms)).ContinueWith(t => EnqueuePlayer(player));
        }

        private void EnqueuePlayer(Player player)
        {
            logger.AddLogItem("Player {0} returning back to the game, players {1}", player.Name, players.Count);
            players.Enqueue(player);
        }

        private async Task ApplyPenalty(int milliseconds, CancellationToken ctoken)
        {
            //ctoken.ThrowIfCancellationRequested();
            await Wait(milliseconds, ctoken);
        }

        private Task Wait(int milliseconds, CancellationToken ctoken)
        {
            ctoken.ThrowIfCancellationRequested();
            return Task.Factory.StartNew(() => Task.Delay(milliseconds), ctoken);
        }
    }
}
