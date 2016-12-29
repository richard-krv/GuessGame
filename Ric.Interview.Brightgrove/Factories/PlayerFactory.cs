using Ric.GuessGame.Presentation;
using System;

namespace Ric.GuessGame.Models
{
    public abstract partial class Player
    {
        public static Player NewPlayer(PlayerType pType, string name, IGameRules rules)
        {
            switch (pType)
            {
                case PlayerType.Random: return new PlayerRandom(name, rules); break;
                case PlayerType.Memory: return new PlayerMemory(name, rules); break;
                case PlayerType.Thorough: return new PlayerThorough(name, rules); break;
                case PlayerType.Cheater: return new PlayerCheater(name, rules); break;
                case PlayerType.ThoroughCheater: return new PlayerThoroughCheater(name, rules); break;
            }
            return null;
        }
        public static Player NewPlayer(IParserPlayer p, IGameRules rules)
        {
            return NewPlayer((PlayerType) Enum.Parse(typeof(PlayerType), p.Type, true) , p.Name, rules);
        }
    }
}
