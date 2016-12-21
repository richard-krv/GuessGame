using Ric.Interview.Brightgrove.FruitBasket.Algorythms;
using Ric.Interview.Brightgrove.FruitBasket.Exceptions;
using System;
using System.Collections.Generic;

namespace Ric.Interview.Brightgrove.FruitBasket.Models
{
    public abstract partial class Player : IGuessGamePlayer
    {
        private readonly string name;
        public string Name { get { return name; } }

        public PlayerType Type
        {
            get
            {
                if (GetType() == typeof(PlayerRandom)) return PlayerType.Random;
                if (GetType() == typeof(PlayerMemory)) return PlayerType.Memory;
                if (GetType() == typeof(PlayerCheater)) return PlayerType.Cheater;
                if (GetType() == typeof(PlayerThoroughCheater)) return PlayerType.ThoroughCheater;
                if (GetType() == typeof(PlayerThorough)) return PlayerType.Thorough;
                return PlayerType.Unknown;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        // constructor
        public Player(string name, IGameRules rules)
        {
            this.name = name;
            this.GameRules = rules;
        }

        // main algorythm action
        public abstract int Guess();

        // game rules
        public IGameRules GameRules;

        // concrete classes
        public class PlayerRandom : Player
        {
            public PlayerRandom(string name, IGameRules rules) : base(name, rules)  { }
            public override int Guess()
            {
                var r = new Random();
                var nextGuess = r.Next(GameRules.MinValue, GameRules.MaxValue);
                return nextGuess;
            }
        }
        public class PlayerMemory : PlayerRandom
        {
            // hash set for faster search
            private HashSet<int> guessHistory;
            public PlayerMemory(string name, IGameRules rules) : base(name, rules)
            {
                guessHistory = new HashSet<int>();
            }
            public override int Guess()
            {
                if (Math.Abs(GameRules.MaxValue - GameRules.MinValue + 1) == guessHistory.Count)
                    throw new AllValuesGuessedException();

                int guess;
                do
                {
                    guess = base.Guess();
                }
                while (guessHistory.Contains(guess));
                guessHistory.Add(guess);

                return guess;
            }
        }
        public class PlayerThorough : Player
        {
            private int lastGuess;
            public PlayerThorough(string name, IGameRules rules) : base(name, rules)
            {
                lastGuess = rules.MinValue;
            }
            public override int Guess()
            {
                return lastGuess++;
            }
        }
        public class PlayerCheater : PlayerRandom, ICheaterPlayer
        {
            private ICheatingAlgorithm cheatAlg;
            public PlayerCheater(string name, IGameRules rules) : base(name, rules) { }
            public override int Guess()
            {
                return cheatAlg.Guess(base.Guess);
            }
        }
        public class PlayerThoroughCheater : PlayerThorough, ICheaterPlayer
        {
            private ICheatingAlgorithm cheatAlg;
            public PlayerThoroughCheater(string name, IGameRules rules) : base(name, rules)  { }
            public override int Guess()
            {
                return cheatAlg.Guess(base.Guess);
            }
        }
    }
}
