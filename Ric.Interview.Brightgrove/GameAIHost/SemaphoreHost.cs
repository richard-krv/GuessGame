using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Concurrent;
using Ric.Interview.Brightgrove.FruitBasket.Extentions;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    public class SemaphoreHost : IGameAIHost
    {
        protected readonly ConcurrentQueue<IGuessGamePlayer> players;
        protected readonly CancellationTokenSource ctSrc;

        public ILogger Logger { get; private set; }

        private IMaintenanceInfo mi = new MaintenanceInfo();
        private readonly IGameResolver resolver;

        public int TotalAttemptsCount { get { return mi.GameGuessHistory.Count; } }

        public IGameOutput GameOutput { get { return mi.GetGameOutput(resolver.SecretValue); } }

        private SemaphoreSlim sem;

        private SemaphoreHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            this.Logger = logger;
            this.resolver = gameResolver;

            // init players
            this.players = playersIncome.ToConcurrentQueue(gameRules, gameResolver, mi);

            // devise a game finish condition
            ctSrc = new CancellationTokenSource(gameResolver.MaxMilliseconds);
            ctSrc.Token.Register(CancellationRoutine);

            sem = new SemaphoreSlim(0, 1);
        }
        private void CancellationRoutine()
        {
            Logger.AddLogItem("Game has been cancelled");
        }

        private CancellationToken token { get { return ctSrc.Token; } }
        public void StartGame()
        {
            try
            {
                while (!token.IsCancellationRequested && mi.GameGuessHistory.Count < resolver.MaxAttempts)
                {
                    IGuessGamePlayer player;
                    if (players.TryDequeue(out player))
                    {
                        var guessVal = player.Guess();
                        mi.AddGuessHistoryItem(guessVal, player);
                        Logger.AddLogItem("Player {0} made a guess {1}", player.Name, guessVal);

                        var penalty = Math.Abs(resolver.SecretValue - guessVal);
                        if (penalty == 0)
                        {
                            Logger.AddLogItem("Player {0} has successfully guessed the secret number", player.Name);
                            ctSrc.Cancel(true);
                        }
                        else
                            Task.Run(async delegate
                            {
                                await ApplyPenalty(penalty, player);
                                players.Enqueue(player);
                                Logger.AddLogItem("Player {0} returning back to the game. Players {1}", player.Name, players.Count);
                                ReleaseMainThread();
                            });
                    }
                    else
                    {
                        Logger.AddLogItem("Empty queue - freezing the main thread --------------------- {0}", players.Count);
                        if (Interlocked.CompareExchange(ref IsSemaphoreWaiting, 1, 0) == 0)
                            sem.Wait(token);
                        Logger.AddLogItem("Semaphore released -------- {0}", players.Count);
                    }
                }
            }
            catch(OperationCanceledException) { }
        }
        private async Task ApplyPenalty(int penaltyMilliseconds, IGuessGamePlayer player)
        {
            Logger.AddLogItem("Player {0} starts waiting.", player.Name);
            await Task.Delay(penaltyMilliseconds, ctSrc.Token);
        }
        // a player should be returned back to the queue before releasing the main thread
        private void ReleaseMainThread()
        {
            if (Interlocked.CompareExchange(ref IsSemaphoreWaiting, 0, 1) == 1)
            {
                Logger.AddLogItem("Releasing semaphore -------");
                sem.Release();
            }
        }
        private int IsSemaphoreWaiting = 0;
        public  void Dispose()
        {
            sem.Dispose();
            ctSrc.Dispose();
        }

        public static IGameAIHost GetGameHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            return new SemaphoreHost(gameRules, gameResolver, playersIncome, logger);
        }
    }
}
