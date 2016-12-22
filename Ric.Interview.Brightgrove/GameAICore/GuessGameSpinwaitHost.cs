using Ric.Interview.Brightgrove.FruitBasket.Models;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using Ric.Interview.Brightgrove.FruitBasket.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ric.Interview.Brightgrove.FruitBasket.GameAICore
{
    internal abstract class GuessGameSpinwaitHost : GuessGameHostBase
    {

        public GuessGameSpinwaitHost(IGameRules gameRules, IGameResolver gameResolver,
            IEnumerable<IParserPlayer> playersIncome, ILogger logger)
                : base(gameRules, gameResolver, playersIncome, logger)
        {
        }

        protected override void InitiateGameStart(CancellationToken ctoken)
        {
            var sw = new SpinWait();
            var spinlog = true;
            while (!ctoken.IsCancellationRequested && GameLog.GuessHistory.Count < resolver.MaxAttempts)
            {
                spinlog = true;
                ctoken.ThrowIfCancellationRequested();
                Player player;
                if (players.TryDequeue(out player))
                {
                    ProcessPlayerGuess(player);
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

        protected abstract void ProcessPlayerGuess(Player player);
    }
}
