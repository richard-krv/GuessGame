using Ric.Interview.Brightgrove.FruitBasket.Extentions;
using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public class GuessGameInlineDelayRuler
    {
        private ConcurrentQueue<Player> players;
        private CancellationTokenSource ctSrc;
        public IGuessGame<int> game { get; private set; }
        private readonly ILogger logger;

        public CancellationToken GameState { get { return ctSrc.Token; } }

        public GuessGameInlineDelayRuler(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
        {
            this.logger = logger;

            var mi = new MaintenanceInfo();
            game = GameFactory.GetGame<int>(gameRules, gameResolver, mi, logger) as IGuessGame<int>;

            // init players
            this.players = playersIncome.ToConcurrentQueue(gameRules, gameResolver, mi);

            // devise a game finish condition
            ctSrc = new CancellationTokenSource(gameResolver.MaxMilliseconds);
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
                foreach (var e in aex.InnerExceptions)
                    logger.AddLogItem(e.Message, e.StackTrace);
            }
            catch (OperationCanceledException)
            {
                logger.AddLogItem("Game has been cancelled");
            }
            catch(Exception)
            {
                logger.AddLogItem("An error has occured duting the game");
                throw;
            }
            finally
            {
                logger.AddLogItem("Exiting game");
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
                    logger.AddLogItem(">> Processing {1}, players in the queue: {0}", players.Count, player.Name);
                    var penalty = game.ValidateGuess(player);
                    if (penalty == 0)
                    {
                        logger.AddLogItem("Player {0} has successfully guessed the secret number", player.Name);

                        ctSrc.Cancel(true);
                    }
                    else
                    {
                        logger.AddLogItem("Player {0} is waiting for {1} seconds", player.Name, penalty / 1000);

                        /*Task.Run(() => Task.Delay(penalty, ctoken)
                            .ConfigureAwait(false).GetAwaiter()
                            .OnCompleted(() => EnqueuePlayer(player))
                            , ctoken);*/
                        Task.Run(async delegate {
                            await Task.Delay(penalty, ctoken);
                            EnqueuePlayer(player);
                            }, ctoken);
                    }
                }
                else
                {
                    logger.AddLogItem("empty queue --------------------------------------");
                    sw.SpinOnce();
                }
            }
        }

        private void EnqueuePlayer(Player player)
        {
            ctSrc.Token.ThrowIfCancellationRequested();
            logger.AddLogItem("Player {0} returning back to the game, players {1}", player.Name, players.Count);
            players.Enqueue(player);
        }

    }
}