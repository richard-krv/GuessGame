using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using Ric.Interview.Brightgrove.FruitBasket.Strategy;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    internal class SemaphoreHost : ObservableGameHost
    {
        public SemaphoreHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
                : base(gameRules, gameResolver, playersIncome, logger)
        {
            //having only one waiting thread and players.Count semaphore.Release() calls
            //we could have up to players.Count - 1 'empty' cycles of TryDequeue->Semaphore.Wait's
            //which is much less than SpinWait
            sem = new SemaphoreSlim(0, players.Count);
        }

        protected override void Game_OnSuccessGuess(Player winPlayer)
        {
            logger.AddLogItem("Player {0} has successfully guessed the secret number", winPlayer.Name);
            ctSrc.Cancel(true);
        }

        protected override async void Game_OnFailedGuess(Player p, int penalty)
        {
            try
            {
                ctSrc.Token.ThrowIfCancellationRequested();
                logger.AddLogItem("Player {0} is waiting {1}", p.Name, penalty);

                await Task.Delay(penalty, ctSrc.Token);

                logger.AddLogItem("Player {0} returning back to the game. Players {1}", p.Name, players.Count);
                players.Enqueue(p);

                //avoiding empty release cycles with the lock
                if (players.Count <= 1)
                {
                    lock (lockobj)
                    {
                        if (players.Count <= 1)
                        {
                            logger.AddLogItem("Releasing semaphore -------");
                            sem.Release();
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        private object lockobj = new object();

        protected override void InitiateGameStart(CancellationToken token)
        {
            while (!token.IsCancellationRequested && GameLog.GuessHistory.Count < resolver.MaxAttempts)
            {
                Player player;
                if (players.TryDequeue(out player))
                {
                    game.ValidateGuess(player);
                }
                else
                {
                    logger.AddLogItem("Empty queue -------------------------------------- {0}", players.Count);
                    sem.Wait(token);
                    logger.AddLogItem("Semaphore released -------- {0}", players.Count);
                }
            }
            ctSrc.Cancel(true);
        }

        private SemaphoreSlim sem;

        public override void Dispose()
        {
            sem.Dispose();
            base.Dispose();
        }
    }
}
