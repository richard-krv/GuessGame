using System;

namespace Ric.GuessGame.Algorythms
{
    public interface ICheatingAlgorithm
    {
        int Guess(Func<int> straightGuess);
    }
}
