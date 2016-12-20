using System;

namespace Ric.Interview.Brightgrove.FruitBasket.Factories.AbstractFactory
{
    public class PlayerFactoryDefault : IPlayerFactory
    {
        IPlayerCheater IPlayerFactory.CreatePlayerCheater(IPlayerConstructorParams prm)
        {
            throw new NotImplementedException();
        }

        IPlayerMemory IPlayerFactory.CreatePlayerMemory(IPlayerConstructorParams prm)
        {
            throw new NotImplementedException();
        }

        IPlayerRandrom IPlayerFactory.CreatePlayerRandrom(IPlayerConstructorParams prm)
        {
            throw new NotImplementedException();
        }

        IPlayerThorough IPlayerFactory.CreatePlayerThorough(IPlayerConstructorParams prm)
        {
            throw new NotImplementedException();
        }

        IPlayerThoroughCheater IPlayerFactory.CreatePlayerThoroughCheater(IPlayerConstructorParams prm)
        {
            throw new NotImplementedException();
        }
    }
}