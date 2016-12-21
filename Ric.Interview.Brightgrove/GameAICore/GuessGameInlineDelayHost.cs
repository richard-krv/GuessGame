using Ric.Interview.Brightgrove.FruitBasket.Factories;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    public class GuessGameInlineDelayHost: GuessGameHostBase
    {
        internal IGuessGame<int> game { get; private set; }
        public override GameLog GameLog { get { return game.GameLog; } }

        public GuessGameInlineDelayHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
                : base(gameRules, gameResolver, playersIncome, logger)
        {
            game = GameFactory.GetGame<int>(gameRules, gameResolver, mi, logger) as IGuessGame<int>;
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

                        Task.Run(async delegate {
                            await Task.Delay(penalty, ctoken);
                            EnqueuePlayer(player);
                            }, ctoken);
                    }
                }
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

        private void EnqueuePlayer(Player player)
        {
            ctSrc.Token.ThrowIfCancellationRequested();
            logger.AddLogItem("Player {0} returning back to the game, players {1}", player.Name, players.Count);
            players.Enqueue(player);
        }

    }
}