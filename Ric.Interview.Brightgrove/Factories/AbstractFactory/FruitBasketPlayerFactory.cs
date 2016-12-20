using Ric.Interview.Brightgrove.FruitBasket.Exceptions;
using Ric.Interview.Brightgrove.FruitBasket.Factories.AbstractFactory;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories
{
    public sealed class FruitBasketPlayerFactory
    {
        public static IList<Player> ParseJson(string players)
        {
            var factory = GetFactory();
            var player = null;
            switch (PlayerType)
            {
                case PlayerType.Randrom: player = factory.CreatePlayerRandrom(); break;
                case PlayerType.Memory: player = factory.CreatePlayerMemory(); break;
                case PlayerType.Thorough: player = factory.CreatePlayerThorough(); break;
                case PlayerType.Cheater: player = factory.CreatePlayerCheater(); break;
                case PlayerType.ThoroughCheater: player = factory.CreatePlayerThoroughCheater(); break;
            }
            obA.foo();
        }

        private static IPlayerFactory GetFactory()
        {
            switch(ConfigurationManager.AppSettings["Player.Factory.Type"])
            {
                case "win_console_player":
                    return new PlayerFactoryDefault();
            }

            throw new IncorrectAbstractFactorySettingsException("Can't recognise an abstract factory settings.");
        }
    }
    
}