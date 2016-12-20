using Ric.Interview.Brightgrove.FruitBasket.Algorythms;
using Ric.Interview.Brightgrove.FruitBasket.Models;
using System.Reflection;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public sealed class CheaterPlayerFactory
    {
        public static ICheaterPlayer InitializeCheaterPlayer(ICheaterPlayer chPlayer,
            ICheatingAlgorithm minfo)
        {
            if (!(chPlayer is ICheaterPlayer)) return null;

            var fld = chPlayer.GetType().GetField(CheatAlgorithmFieldName,
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.GetField);
            fld.SetValue(chPlayer, minfo);

            return chPlayer;
        }

        public const string CheatAlgorithmFieldName = "cheatAlg";
    }
}
