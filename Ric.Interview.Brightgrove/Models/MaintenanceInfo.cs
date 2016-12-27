using Ric.Interview.Brightgrove.FruitBasket.Exceptions;
using Ric.Interview.Brightgrove.FruitBasket.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    internal class MaintenanceInfo : IMaintenanceInfo
    {
        public HashSet<int> GameGuessHistory { get; private set; }
        private List<GuessHistoryLogRecord> GuessHistoryLog { get; set; }
        public MaintenanceInfo()
        {
            GameGuessHistory = new HashSet<int>();
            GuessHistoryLog = new List<GuessHistoryLogRecord>();
        }

        public void AddGuessHistoryItem(int value, IGuessGamePlayer player)
        {
            GuessHistoryLog.Add(new GuessHistoryLogRecord(player, value));
            GameGuessHistory.Add(value);
        }

        public bool Contains(int value)
        {
            return GameGuessHistory.Contains(value);
        }

        public IGameOutput GetGameOutput(int secretValue)
        {
            var winlogrec = GetWinnerLogRecord(secretValue);

            return new GameOutput(
                secretValue,
                winlogrec.Player,
                winlogrec.GuessValue,
                GetNumberOfAttempts(winlogrec.Player));
        }

        private GuessHistoryLogRecord GetWinnerLogRecord(int secretValue)
        {
            // if there's a clear winner player
            if (this.Contains(secretValue))
            {
                var lastRecord = GuessHistoryLog.Last();
                if (lastRecord.GuessValue == secretValue)
                    return lastRecord;
                else
                    throw new InvalidGameCompletionException("Invalid winner player");
            }
            else
            {
                // find the first closest guess player
                var minDif = GuessHistoryLog.Min(e => Math.Abs(e.GuessValue - secretValue));
                return GuessHistoryLog.First(e => 
                    Math.Abs(e.GuessValue - secretValue) == minDif);
            }
        }

        private int GetNumberOfAttempts(IGuessGamePlayer player)
        {
            return GuessHistoryLog.Count(e => e.Player.Equals(player));
        }

        private class GuessHistoryLogRecord
        {
            public readonly IGuessGamePlayer Player;
            public readonly int GuessValue;
            public GuessHistoryLogRecord(IGuessGamePlayer player, int guess)
            {
                Player = player;
                GuessValue = guess;
            }
        }
    }
}
