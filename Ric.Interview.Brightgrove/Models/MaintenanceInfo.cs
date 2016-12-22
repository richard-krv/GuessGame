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

        public void AddGuessHistoryItem(int value, Player player)
        {
            GuessHistoryLog.Add(new GuessHistoryLogRecord(player, value));
            GameGuessHistory.Add(value);
        }

        public bool Contains(int value)
        {
            return GameGuessHistory.Contains(value);
        }

        public Player GetWinnerPlayer(int secretValue)
        {
            Player player;
            // if there's a winner player
            if (this.Contains(secretValue))
            {
                var lastRecord = GuessHistoryLog.Last();
                if (lastRecord.GuessValue == secretValue)
                    player = lastRecord.Player;
                else
                    throw new InvalidGameCompletionException("Invalid winner player");
            }
            else
            {
                // find the first closest guess player
                var minDif = GuessHistoryLog.Min(e => Math.Abs(e.GuessValue - secretValue));
                player = GuessHistoryLog.First(e => 
                    Math.Abs(e.GuessValue - secretValue) == minDif).Player;
            }
            return player;
        }

        public int GetNumberOfAttempts(Player player)
        {
            return GuessHistoryLog.Count(e => e.Player.Equals(player));
        }

        private class GuessHistoryLogRecord
        {
            public readonly Player Player;
            public readonly int GuessValue;
            public GuessHistoryLogRecord(Player player, int guess)
            {
                Player = player;
                GuessValue = guess;
            }
        }
    }
}
