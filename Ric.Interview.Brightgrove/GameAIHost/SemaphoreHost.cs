using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ric.GuessGame.Models;
using Ric.GuessGame.Presentation;
using Ric.GuessGame.Utils;
using System.Collections.Concurrent;
using Ric.GuessGame.Extentions;

namespace Ric.GuessGame.GameAICore
{
    internal class SemaphoreHost : IGameAIHost
    {
        private readonly ConcurrentQueue<IGuessGamePlayer> players;
        private readonly CancellationTokenSource ctSrc;
        private readonly CancellationTokenRegistration reg;

        public ILogger Logger { get; private set; }

        private readonly IMaintenanceInfo mi = new MaintenanceInfo();
        private readonly IGameResolver resolver;

        public int TotalAttemptsCount { get { return mi.TotalAttemptsCount; } }

        public IGameOutput GameOutput { get { return mi.GetGameOutput(resolver.SecretValue); } }

        private SemaphoreSlim sem;

        private SemaphoreHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            Logger = logger;
            resolver = gameResolver;

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
                Logger.AddLogItem("Starting the game");
                while (!token.IsCancellationRequested && mi.TotalAttemptsCount < resolver.MaxAttempts)
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
                                token.ThrowIfCancellationRequested();
                                players.Enqueue(player);
                                Logger.AddLogItem("Player {0} returning back to the game. Players {1}", player.Name, players.Count);
                                ReleaseMainThread();
                            });
                    }
                    else
                    {
                        Logger.AddLogItem("Empty queue - freezing the main thread ---------------------");
                        if (Interlocked.CompareExchange(ref IsSemaphoreWaiting, 1, 0) == 0)
                            sem.Wait(token);
                        Logger.AddLogItem("Semaphore released -------- {0}", players.Count);
                    }
                }
            }
            catch(OperationCanceledException) { }
            finally
            {
                Logger.AddLogItem("Game over");
            }
        }
        private async Task ApplyPenalty(int penaltyMilliseconds, IGuessGamePlayer player)
        {
            token.ThrowIfCancellationRequested();
            Logger.AddLogItem("Player {0} starts waiting.", player.Name);
            await Task.Delay(penaltyMilliseconds, token);
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
        private volatile int IsSemaphoreWaiting = 0;
        public  void Dispose()
        {
            sem.Dispose();
            reg.Dispose();
            ctSrc.Dispose();
        }

        public static IGameAIHost GetGameHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            return new SemaphoreHost(gameRules, gameResolver, playersIncome, logger);
        }
    }
}
