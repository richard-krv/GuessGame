using Ric.Interview.Brightgrove.FruitBasket.Extentions;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    public abstract class GuessGameHostBase: IGameAIHost
    {
        protected readonly ConcurrentQueue<Player> players;
        protected readonly CancellationTokenSource ctSrc;

        protected readonly ILogger logger;
        internal protected IMaintenanceInfo mi = new MaintenanceInfo();
        internal readonly protected IGameResolver resolver;

        public bool IsCancellationRequested { get { return ctSrc.Token.IsCancellationRequested; } }
        public abstract GameLog GameLog { get; }
        public IGameOutput GameOutput { get; private set; }

        public GuessGameHostBase(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            this.logger = logger;
            this.resolver = gameResolver;

            // init players
            this.players = playersIncome.ToConcurrentQueue(gameRules, gameResolver, mi);

            // devise a game finish condition
            ctSrc = new CancellationTokenSource(gameResolver.MaxMilliseconds);
            ctSrc.Token.Register(CancellationRoutine);

            //come up with some output
            GameOutput = new GameOutput(gameResolver, mi);
        }

        private void CancellationRoutine()
        {
            logger.AddLogItem("Game has been cancelled");
        }

        public void StartGame()
        {
            try
            {
                logger.AddLogItem("Game started");
                InitiateGameStart(ctSrc.Token);
                logger.AddLogItem("Game finished");
            }
            catch (AggregateException aex)
            {
                logger.AddLogItem("Aggregate exception has occured {0}, {1}, count: {2}",
                    aex.Message, aex.StackTrace, aex.InnerExceptions.Count);
                aex.Flatten().Handle(e =>
                {
                    if (!(e is OperationCanceledException))
                        logger.AddLogItem(e.Message, e.StackTrace);
                    return true;
                });
            }
            catch (OperationCanceledException)
            {
                logger.AddLogItem("Game has been cancelled");
            }
            finally
            {
                logger.AddLogItem("Exiting game");
            }
        }

        protected abstract void InitiateGameStart(CancellationToken token);

        //simplified dispose pattern
        //subclasses should override and call base dispose
        public virtual void Dispose()
        {
            ctSrc.Dispose();
        }

        public IGameOutput GetGameOutput()
        {
            return GameOutput;
        }
    }
}
