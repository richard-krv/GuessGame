﻿using Ric.Interview.Brightgrove.FruitBasket.Models;
using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Algorythms
{
    public class CheatPippingGuessHistory: ICheatingAlgorithm
    {
        private IMaintenanceInfo mi;
        public CheatPippingGuessHistory(IMaintenanceInfo mi)
        {
            this.mi = mi;
        }

        public int Guess(Func<int> straightGuess)
        {
            int guess;
            do
            {
                guess = straightGuess();
            } while (mi.GameGuessHistory.Contains(guess));
            return guess;
        }
    }
}
